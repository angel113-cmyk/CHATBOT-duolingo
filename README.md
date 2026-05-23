# Base de Chatbot estilo Duolingo

Proyecto inicial para un chatbot con diseño inspirado en Duolingo.

## Qué incluye

- `public/index.html` - interfaz principal con diseño moderno.
- `public/styles.css` - estilo de la página.
- `public/script.js` - lógica de frontend para enviar mensajes.
- `server.js` - servidor Express.
- `routes/chatController.js` - controlador de chat.

## Cómo empezar

1. Abre una terminal en `c:\Users\User\Downloads\Duolingo`
2. Ejecuta `npm install`
3. Ejecuta `npm start`
4. Abre `http://localhost:3000`

## Cómo funciona

- El frontend envía POST a `/api/chat`
- `routes/chatController.js` procesa el texto y devuelve una respuesta JSON
- `public/script.js` muestra los mensajes en la UI

## Próximos pasos

- Añadir lógica de IA real o integraciones con APIs de lenguaje.
- Mejorar el diseño y el flujo de conversación.
- Guardar conversaciones y estados de usuario.
