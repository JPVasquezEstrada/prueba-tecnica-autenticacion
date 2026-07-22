(function () {
    const TIEMPO_TOTAL_MIN = 1;
    const AVISO_ANTES_SEG = 10;

    const tiempoTotalMs = TIEMPO_TOTAL_MIN * 60 * 1000;
    const tiempoAvisoMs = tiempoTotalMs - (AVISO_ANTES_SEG * 1000);

    let timerAviso, timerExpira, intervaloCuenta;
    let segundosRestantes = AVISO_ANTES_SEG;

    function iniciarTimers() {
        clearTimeout(timerAviso);
        clearTimeout(timerExpira);
        clearInterval(intervaloCuenta);

        timerAviso = setTimeout(mostrarModalAviso, tiempoAvisoMs);
    }

    function mostrarModalAviso() {
        segundosRestantes = AVISO_ANTES_SEG;
        document.getElementById('modalExpiracion').style.display = 'flex';
        actualizarContador();

        intervaloCuenta = setInterval(() => {
            segundosRestantes--;
            actualizarContador();
            if (segundosRestantes <= 0) {
                clearInterval(intervaloCuenta);
                expirarSesion();
            }
        }, 1000);
    }

    function actualizarContador() {
        document.getElementById('contadorSegundos').textContent = segundosRestantes;
    }

    function expirarSesion() {
        window.location.href = '/Account/SesionExpirada';
    }

    window.extenderSesion = function () {
        fetch('/Account/Ping').then(() => {
            document.getElementById('modalExpiracion').style.display = 'none';
            clearInterval(intervaloCuenta);
            iniciarTimers();
        });
    };

    // Cualquier actividad real del usuario reinicia el conteo,
    // PERO solo si el modal de aviso no está visible todavía
    ['mousemove', 'keydown', 'click', 'scroll'].forEach(evento => {
        document.addEventListener(evento, () => {
            const modalVisible = document.getElementById('modalExpiracion').style.display === 'flex';
            if (!modalVisible) {
                iniciarTimers();
            }
        });
    });

    iniciarTimers();
})();