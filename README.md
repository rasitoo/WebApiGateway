# WebApiGateway

Gateway API construido con **.NET 8** y **Ocelot** para enrutar solicitudes a microservicios backend. Incluye soporte para CORS, Swagger y WebSockets.

## Características

- Enrutamiento de API mediante Ocelot.
- Soporte para WebSockets, permitiendo la notificación de cambios en tiempo real gracias a SignalR.
- Documentación interactiva con Swagger UI.
- Configuración de CORS para desarrollo local (React u otros frontends).
- Preparado para despliegue con Docker.

## Configuración

### 1. Variables de entorno

Puedes configurar variables de entorno en el archivo `appsettings.json` 

### 2. Configuración de Ocelot

Define las rutas y reglas de proxy en `ocelot.json`.  
Para habilitar WebSockets en una ruta, añade `"UseWebSockets": true`.

Ejemplo:

```json
{
  "DownstreamPathTemplate": "/ws",
  "DownstreamScheme": "ws",
  "DownstreamHostAndPorts": [
    { "Host": "localhost", "Port": 5001 }
  ],
  "UpstreamPathTemplate": "/ws",
  "UpstreamHttpMethod": [ "GET" ],
  "UseWebSockets": true
}

```

- **DownstreamPathTemplate**:  
  Es la ruta a la que Ocelot enviará la solicitud en el servicio backend (microservicio). En este caso, `/ws`.

- **DownstreamScheme**:  
  El esquema de la conexión hacia el backend. Para WebSockets debe ser `"ws"` (o `"wss"` si es seguro).

- **DownstreamHostAndPorts**:  
  Lista de hosts y puertos donde está corriendo el backend que recibirá la conexión WebSocket.  
  Aquí, el backend está en `localhost` y puerto `5001`.

- **UpstreamPathTemplate**:  
  Es la ruta por la que el cliente se conecta al gateway.  
  Cuando el cliente se conecta a `/ws` en el gateway, Ocelot redirige la conexión al backend definido arriba.

- **UpstreamHttpMethod**:  
  Métodos HTTP permitidos para esta ruta. Para WebSockets, normalmente es `GET`.

- **UseWebSockets**:  
  Indica a Ocelot que esta ruta debe manejar conexiones WebSocket y no solo HTTP tradicional.

### 3. CORS

Por defecto, solo permite solicitudes desde `http://localhost:5173`.  
Modifica la política en `Program.cs` si necesitas otros orígenes.

## Ejecución local

1. **Restaurar dependencias:**
   
```
   dotnet restore
   
```

2. **Ejecutar el gateway:**
   
```
   dotnet run --project WebApiGateway
   
```

3. **Acceder a Swagger UI:**
   - Navega a `http://localhost:5000/swagger` (ajusta el puerto según tu configuración).

## Uso con Docker

1. **Construir la imagen:**
   
```
   docker build -t webapigateway -f WebApiGateway/Dockerfile .
   
```

2. **Ejecutar el contenedor:**
   
```
   docker run -p 5000:80 webapigateway
   
```

O usa `docker-compose` si tienes un archivo de orquestación.

## Notas

- Asegúrate de que los microservicios backend estén accesibles desde el gateway.
- Si usas HTTPS o diferentes puertos, actualiza la configuración de CORS y Ocelot según corresponda.

## Ejemplo de Docker Compose
```

services:
  webapigateway:
    image: rasito/webapigatewayserver:latest
    container_name: webapigateway
    ports:
      - 25003:8080
    networks:
      - backend
  postgres_messages:
    image: postgres:latest
    container_name: postgres_messages
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: postgres
    volumes:
      - postgres_messages:/var/lib/postgresql/data
    networks:
      - messages
  web-api-messages:
    image: rasito/webapimessages:latest
    container_name: webapimessages
    environment:
      DbSettings__Host: postgres_messages
      DbSettings__Port: 5432
      DbSettings__Username: postgres
      DbSettings__Password: postgres
      DbSettings__Database: postgres
      JwtSettings__SecretKey: 
    depends_on:
      - postgres_messages
    networks:
      - messages
      - backend
  auth_database:
    image: postgres:latest
    container_name: database_authentication
    environment:
      POSTGRES_USER: common
      POSTGRES_PASSWORD: auth
      POSTGRES_DB: Authapi
    volumes:
      - postgres_auth_data:/var/lib/postgresql/data
    networks:
      - authentication
  auth_api:
    image: chema00/authapi:latest
    container_name: authentication_api
    environment:
      DbSettings__Host: auth_database
      DbSettings__Port: 5432
      DbSettings__Username: common
      DbSettings__Password: auth
      DbSettings__Database: Authapi
      JwtSettings__SecretKey: 
      EmailSettings__UserEmail: 
      EmailSettings__UserName: 
      EmailSettings__UserApiKey: 
      EmailSettings__Host: 
    depends_on:
      - auth_database
    networks:
      - authentication
      - backend
  com_database:
    image: postgres:latest
    container_name: database_community
    environment:
      POSTGRES_USER: common
      POSTGRES_PASSWORD: com
      POSTGRES_DB: Comapi
    volumes:
      - postgres_com_data:/var/lib/postgresql/data
    networks:
      - communities
  com_api:
    image: chema00/comunapi:latest
    container_name: community_api
    environment:
      DbSettings__Host: com_database
      DbSettings__Port: 5432
      DbSettings__Username: common
      DbSettings__Password: com
      DbSettings__Database: Comapi
      JwtSettings__SecretKey: 
    depends_on:
      - com_database 
    volumes:
      - community_images:/app/community_images
    networks:
      - communities
      - backend
  prof_database:
    image: postgres:latest
    container_name: database_prof
    environment:
      POSTGRES_USER: prof
      POSTGRES_PASSWORD: prof
      POSTGRES_DB: Prof
    volumes:
      - postgres_prof_data:/var/lib/postgresql/data
    networks:
      - prof
  prof_api:
    image: chema00/profapi:latest
    container_name: prof_api
    environment:
      DbSettings__Host: prof_database
      DbSettings__Port: 5432
      DbSettings__Username: prof
      DbSettings__Password: prof
      DbSettings__Database: Prof
      JwtSettings__SecretKey: 
    depends_on:
      - prof_database 
    volumes:
      - profile_images:/app/profile_images
    networks:
      - prof
      - backend
  rev_database:
    image: postgres:latest
    container_name: database_rev
    environment:
      POSTGRES_USER: rev
      POSTGRES_PASSWORD: rev
      POSTGRES_DB: Rev
    volumes:
      - postgres_rev_data:/var/lib/postgresql/data
    networks:
      - rev
  rev_api:
    image: chema00/revapi:latest
    container_name: rev_api
    environment:
      DbSettings__Host: rev_database
      DbSettings__Port: 5432
      DbSettings__Username: rev
      DbSettings__Password: rev
      DbSettings__Database: Rev
      JwtSettings__SecretKey: 
    depends_on:
      - rev_database
    networks:
      - rev
      - backend
  mongodb:
    image: mongo:4.2.18
    container_name: mongo
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: example
      MONGO_INITDB_DATABASE: mydatabase
    volumes:
      - mongo_workshop_data:/data/db
    networks:
      - workshop
  webapitaller:
    image: rasito/webapiworkshop:latest
    container_name: workshop_api
    depends_on:
      - mongodb
    environment:
      DbSettings__Host: mongo
      DbSettings__Port: 27017
      DbSettings__Username: root
      DbSettings__Password: example
      DbSettings__Database: mydatabase
      JwtSettings__SecretKey: 
    networks:
      - workshop
      - backend
  web_frontend:
      image: chema00/gearhub:latest
      container_name: webfrontend
      ports:
        - 25002:80
      networks:
        - backend
networks:
  backend:
  messages:
  authentication:
  communities:
  prof:
  rev:
  workshop:
volumes:
  postgres_messages:
  postgres_auth_data:
  postgres_com_data:
  postgres_prof_data:
  postgres_rev_data:
  mongo_workshop_data:
  profile_images:
  community_images:

```

## Licencia

Este proyecto está bajo la licencia MIT.
