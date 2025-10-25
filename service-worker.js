const CACHE_NAME = 'app-v1';
const PRECACHE_URLS = [
    'css/format.css',
    'fonts/EBGaramond-VariableFont_wght.woff',
    'fonts/NotoSansGeorgian-VariableFont.woff',
    'fonts/PlayfairDisplay-VariableFont_wght.woff',
    'images/icons/icon512_maskable.png',
    'images/icons/icon512_rounded.png',
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => cache.addAll(PRECACHE_URLS))
    );
    // Do NOT call skipWaiting here so the new SW goes to 'waiting' and the page can prompt the user.
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(keys.map(k => k !== CACHE_NAME ? caches.delete(k) : Promise.resolve()))
        )
    );
    // Become available to all pages immediately after activation
    event.waitUntil(self.clients.claim().then(() => {
        // Notify clients that a new version is active (useful if you want immediate reload)
        broadcast({ type: 'NEW_VERSION_ACTIVATED' });
    }));
});

self.addEventListener('fetch', event => {
    const req = event.request;

    // Only handle GET same-origin requests
    if (req.method !== 'GET' || new URL(req.url).origin !== location.origin) return;

    // Navigation requests (SPA / page loads) -> network-first with cache fallback
    if (req.mode === 'navigate') {
        event.respondWith(
            fetch(req).then(resp => {
                // Optionally update cache for navigation responses
                const clone = resp.clone();
                caches.open(CACHE_NAME).then(cache => cache.put(req, clone));
                return resp;
            }).catch(() => caches.match('/'))
        );
        return;
    }

    // For other requests: cache-first, then network and cache response
    event.respondWith(
        caches.match(req).then(cached => {
            if (cached) return cached;
            return fetch(req).then(resp => {
                // Don't cache opaque or error responses blindly
                if (!resp || resp.status !== 200 || resp.type === 'opaque') return resp;
                const respClone = resp.clone();
                caches.open(CACHE_NAME).then(cache => {
                    try { cache.put(req, respClone); } catch (e) { /* ignore quota errors */ }
                });
                return resp;
            }).catch(() => {
                // final fallback: try serving root
                return caches.match('/');
            });
        })
    );
});

// Listen for message from the page to skip waiting (activate new SW now)
self.addEventListener('message', event => {
    if (!event.data) return;
    if (event.data.type === 'SKIP_WAITING') {
        self.skipWaiting();
    }
});

// Broadcast helper
function broadcast(message) {
    self.clients.matchAll({ type: 'window', includeUncontrolled: true }).then(clients => {
        for (const client of clients) {
            try { client.postMessage(message); } catch (e) { /* ignore */ }
        }
    });
}
