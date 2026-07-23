# Prueba Tecnica - Autenticacion y Gestion de Sesion

Este es mi proyecto. Hice el login, el bloqueo de cuenta por intentos fallidos, la expiracion de sesion por inactividad y la edicion de perfil

## Con que lo hice

- ASP.NET Core (.NET 10) con C#, usando el patron MVC
- Entity Framework Core para conectar con la base de datos
- SQL Server LocalDB (la base de datos que viene con Visual Studio)
- ASP.NET Identity para todo el tema de login y usuarios
- Bootstrap 5 (por CDN) para los estilos, mas un poco de CSS propio
- jQuery Validation para que los formularios avisen los errores

## Como levantarlo

1. Clonar el repo o descargar el .zip
2. Abrir el `.slnx` en Visual Studio 2026
3. Dejar que restaure los paquetes NuGet
4. Abrir la Consola del Administrador de paquetes (Herramientas > Administrador de paquetes NuGet > Consola del Administrador de paquetes) y correr:

```
Update-Database
```

Con eso se crea la base de datos (`EnchufateAuthDb`) usando las migraciones que ya estan en el proyecto.

5. Correr la aplicacion.

La cadena de conexion esta en `appsettings.json`, usa LocalDB asi que no pide usuario ni contraseña de SQL

```
Server=(localdb)\mssqllocaldb;Database=EnchufateAuthDb;Trusted_Connection=True;MultipleActiveResultSets=true
```

## Usuario para probar

Como el enunciado dice que el registro de cuenta queda fuera del alcance, hice que se cree un usuario automaticamente la primera vez que corres el proyecto (esta en `Data/DbSeeder.cs`, se llama desde `Program.cs`). Es este:

- Usuario: `admin`
- Contraseña: `Admin123`

## Que hice para cada requerimiento

**Login:** pantalla con usuario y contraseña, con boton de mostrar/ocultar la contraseña. Primero valida que los campos no esten vacios, despues revisa si la cuenta esta bloqueada, y recien ahi compara las credenciales.

**Intentos fallidos:** cada vez que fallas usuario o contraseña, sale el mensaje en rojo debajo del campo correspondiente ("Usuario incorrecto" o "Contraseña incorrecta"). El contador de intentos no lo hice yo a mano, lo maneja Identity automaticamente con `PasswordSignInAsync` cuando le pasas `lockoutOnFailure: true`.

**Bloqueo:** a los 5 intentos fallidos se bloquea la cuenta 15 minutos. Esto tambien lo configura Identity, en `Program.cs` dentro de `AddIdentity`, con `options.Lockout`. Cuando se bloquea, simulo con un `Console.WriteLine` en el controlador.

**Perfil:** al loguearte bien, te manda a `/Perfil` con los datos del usuario.

**Editar perfil:** desde el perfil hay un boton Editar que te lleva a un formulario, con Guardar y Cancelar. Al guardar valida los campos (por ejemplo que el correo tenga formato valido) y actualiza los datos en la base.

**Expiracion de sesion:** configure la cookie de la sesion para que dure 20 minutos, con `SlidingExpiration = true` (asi se resetea el tiempo cada vez que haces algo, no es un limite fijo desde que entraste). Con JavaScript arme un timer que, 45 segundos antes de que se cumplan los 20 minutos, muestra un modal preguntando si sigues ahi, con cuenta regresiva. Si no le das a "Extender sesion", te saca y te manda al login con el aviso de que la sesion expiro por inactividad.

## Algunas cosas que decidi por mi cuenta

- Use ASP.NET Identity en vez de armar la tabla de usuarios y toda la validacion a mano, me parecio mejor usar algo ya probado que reinventarlo.
- La clase `Usuario` hereda de `IdentityUser` y le agregue nada mas los campos que necesitaba para el perfil (Nombres, PrimerApellido, SegundoApellido, FechaNacimiento), porque lo de intentos fallidos y bloqueo ya vienen incluidos en Identity.
- Al principio me paso que la hora del bloqueo no coincidia con mi hora real - resulta que Identity guarda la fecha en UTC. Lo solucione convirtiendola a la hora de Peru solo al mostrarla en pantalla, sin tocar como la guarda internamente.


## Estructura del proyecto

```
Controllers/
  AccountController.cs   -> login, logout, bloqueo
  PerfilController.cs    -> ver y editar perfil
  ErrorController.cs     -> pantalla de error generica
Models/
  Usuario.cs              -> hereda de IdentityUser
  PerfilViewModel.cs       -> lo que se edita en el perfil
  LoginViewModel.cs        -> lo que se manda desde el form de login
Data/
  ApplicationDbContext.cs
  DbSeeder.cs              -> crea el usuario admin al arrancar
Views/
  Account/                 -> Login, Bloqueado
  Perfil/                  -> Index, Editar
  Shared/_ModalExpiracion.cshtml
wwwroot/
  css/, js/, images/
Migrations/                -> para reconstruir la base de datos con Update-Database
```
