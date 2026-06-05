using Android.Hardware.Biometrics;
using GeoVial.Sync;
using Microsoft.Maui.ApplicationModel;

namespace GeoVial.Mobile;

/// <summary>
/// Verificación de identidad con el método de seguridad nativo del teléfono (RN-06, S53) usando el
/// <c>BiometricPrompt</c> de la plataforma (huella/rostro/PIN; API 28+). Es la implementación de
/// <see cref="IAutenticadorBiometrico"/>; la decisión de reingreso vive en <see cref="CoordinadorReingreso"/>
/// (en el gate). En dispositivos sin método configurado o sin soporte, devuelve <see cref="ResultadoBiometrico.NoDisponible"/>.
/// Fuera de CI (cáscara MAUI); se verifica on-device.
/// </summary>
public sealed class AutenticadorBiometricoAndroid : IAutenticadorBiometrico
{
    public Task<bool> HayMetodoDisponibleAsync()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(28))
        {
            return Task.FromResult(false);
        }

        var teclado = (Android.App.KeyguardManager?)Android.App.Application.Context
            .GetSystemService(Android.Content.Context.KeyguardService);
        // IsDeviceSecure: el teléfono tiene huella/PIN/patrón configurado (precondición del método, RN-06).
        return Task.FromResult(teclado?.IsDeviceSecure ?? false);
    }

    public Task<ResultadoBiometrico> AutenticarAsync(string motivo, CancellationToken ct = default)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(28))
        {
            return Task.FromResult(ResultadoBiometrico.NoDisponible);
        }

        if (Platform.CurrentActivity is not { } actividad || actividad.MainExecutor is not { } ejecutor)
        {
            return Task.FromResult(ResultadoBiometrico.NoDisponible);
        }

        var tcs = new TaskCompletionSource<ResultadoBiometrico>();
        var prompt = new BiometricPrompt.Builder(actividad)
            .SetTitle("GeoVial — Reingreso en terreno")
            .SetDescription(motivo)
            .SetNegativeButton("Usar clave", ejecutor, new CancelarListener(tcs))
            .Build();

        prompt.Authenticate(new Android.OS.CancellationSignal(), ejecutor, new Callback(tcs));
        return tcs.Task;
    }

    // El sistema invoca estos callbacks en el hilo del executor; resuelven la verificación una sola vez.
    private sealed class Callback : BiometricPrompt.AuthenticationCallback
    {
        private readonly TaskCompletionSource<ResultadoBiometrico> _tcs;
        public Callback(TaskCompletionSource<ResultadoBiometrico> tcs) => _tcs = tcs;

        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult? result) =>
            _tcs.TrySetResult(ResultadoBiometrico.Exito);

        public override void OnAuthenticationError(BiometricErrorCode errorCode, Java.Lang.ICharSequence? errString)
        {
            // 5 = Canceled (por el sistema), 10 = UserCanceled, 13 = NegativeButton.
            var cancelado = (int)errorCode is 5 or 10 or 13;
            _tcs.TrySetResult(cancelado ? ResultadoBiometrico.Cancelado : ResultadoBiometrico.Fallo);
        }

        // OnAuthenticationFailed (intento no reconocido) es transitorio: el sistema reintenta o termina con error.
    }

    private sealed class CancelarListener : Java.Lang.Object, Android.Content.IDialogInterfaceOnClickListener
    {
        private readonly TaskCompletionSource<ResultadoBiometrico> _tcs;
        public CancelarListener(TaskCompletionSource<ResultadoBiometrico> tcs) => _tcs = tcs;
        public void OnClick(Android.Content.IDialogInterface? dialog, int which) =>
            _tcs.TrySetResult(ResultadoBiometrico.Cancelado);
    }
}
