(function () {
  // Robust Swagger UI injector: tries multiple selectors and falls back to top-right corner.
  function findContainer() {
    const selectors = [
      '.swagger-ui .topbar .download-url-wrapper',
      '.swagger-ui .topbar',
      '.swagger-topbar',
      '.swagger-ui',
      '#swagger-ui'
    ];

    for (const sel of selectors) {
      const el = document.querySelector(sel);
      if (el) return el;
    }
    return document.body;
  }

  function createPanel() {
    if (document.getElementById('custom-auth-panel')) return;

    const panel = document.createElement('div');
    panel.id = 'custom-auth-panel';
    panel.style.display = 'flex';
    panel.style.alignItems = 'center';
    panel.style.gap = '8px';
    panel.style.marginLeft = '8px';

    const input = document.createElement('input');
    input.id = 'custom-auth-input';
    input.placeholder = 'Bearer <token>';
    input.style.width = '420px';

    const setBtn = document.createElement('button');
    setBtn.textContent = 'Set Authorization';

    const clearBtn = document.createElement('button');
    clearBtn.textContent = 'Clear';

    setBtn.onclick = function () {
      const val = input.value.trim();
      if (val) {
        localStorage.setItem('swagger_custom_auth', val);
        window.__custom_swagger_auth = val;
        console.info('[swagger-init] Authorization set');
        alert('Authorization header set');
      }
    };

    clearBtn.onclick = function () {
      localStorage.removeItem('swagger_custom_auth');
      window.__custom_swagger_auth = null;
      input.value = '';
      console.info('[swagger-init] Authorization cleared');
      alert('Authorization cleared');
    };

    panel.appendChild(input);
    panel.appendChild(setBtn);
    panel.appendChild(clearBtn);

    // restore
    const saved = localStorage.getItem('swagger_custom_auth');
    if (saved) {
      input.value = saved;
      window.__custom_swagger_auth = saved;
    }

    return panel;
  }

  function injectPanel() {
    try {
      const target = findContainer();
      if (!target) return;

      // place panel inside target; if topbar, append to it, otherwise append to body and position fixed
      const panel = createPanel();
      if (target === document.body) {
        panel.style.position = 'fixed';
        panel.style.top = '8px';
        panel.style.right = '8px';
        panel.style.background = 'rgba(255,255,255,0.95)';
        panel.style.padding = '6px';
        panel.style.borderRadius = '6px';
        panel.style.boxShadow = '0 2px 6px rgba(0,0,0,0.2)';
        document.body.appendChild(panel);
      } else {
        // insert at start so it appears on left side of topbar
        if (!target.contains(panel)) target.insertBefore(panel, target.firstChild);
      }
    } catch (e) {
      console.error('[swagger-init] inject error', e);
    }
  }

  function patchFetchAndXhr() {
    if (window.__swagger_auth_patched) return;

    // patch fetch
    if (window.fetch) {
      const originalFetch = window.fetch.bind(window);
      window.fetch = function (input, init) {
        init = init || {};
        init.headers = init.headers || {};
        try {
          const auth = window.__custom_swagger_auth;
          if (auth) {
            if (init.headers instanceof Headers) init.headers.set('Authorization', auth);
            else if (Array.isArray(init.headers)) init.headers.push(['Authorization', auth]);
            else init.headers['Authorization'] = auth;
          }
        } catch (e) {
          console.warn('[swagger-init] fetch header set failed', e);
        }
        return originalFetch(input, init);
      };
    }

    // patch XHR
    (function () {
      const origSend = XMLHttpRequest.prototype.send;
      XMLHttpRequest.prototype.send = function () {
        try {
          const auth = window.__custom_swagger_auth;
          if (auth && this.setRequestHeader) {
            this.setRequestHeader('Authorization', auth);
          }
        } catch (e) {}
        return origSend.apply(this, arguments);
      };
    })();

    window.__swagger_auth_patched = true;
  }

  function init() {
    injectPanel();
    patchFetchAndXhr();
  }

  const observer = new MutationObserver(() => {
    init();
  });
  observer.observe(document.body, { childList: true, subtree: true });

  // also try once after short delay
  setTimeout(init, 1200);
})();
