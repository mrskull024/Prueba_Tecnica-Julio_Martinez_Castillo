#  Prueba Técnica- Desarrollador .NET Core, C# Vue 3 para Unilink 

## Descripción del proyecto

1. `PruebaTec.Models`: Contiene los modelos que usa el API.
2. `PruebaTec.Data`: Contiene la logica de acceso a datos.
3. `PruebaTec.API (o WebApplication1)`: Contiene el API con los endpoints de Usuario y Ruleta.
4. `pruebatec.site`: Contiene el front end realizado con vue y vite.
5. `Scripts`: Contiene el query necesario para crear la base de dato, la tabla y los procedimientos almacenados necesarios para el funcionamiento del API

### Deberá tener en consideración lo siguiente para ejecutar en local:
1. Cambiar la cadena de conexión DefaultConnection en `appSettings.json`
2. Ejecutar en SQL Server `Query.sql` que esta en la carpeta `Scripts`
3. El API esta bajo `NET 8` por lo que si no esta instalado en la maquina podrá descargarlo de [NET FRAMEWORK 8](https://dotnet.microsoft.com/es-es/download/dotnet/8.0)
