const express = require('express');
const router = express.Router();

router.post('/', (req, res) => {
  const message = req.body.message || '';
  const reply = generateReply(message);
  res.json({ reply });
});

function generateReply(message) {
  const text = message.trim().toLowerCase();
  if (!text) {
    return 'Escribe algo para comenzar a chatear 😊';
  }
  if (/hola|buenas|hey/.test(text)) {
    return '¡Hola! Soy tu asistente para practicar. ¿En qué tema quieres trabajar hoy?';
  }
  if (/gracias|thank/.test(text)) {
    return '¡De nada! Si quieres, puedo ayudarte a practicar vocabulario o frases.';
  }
  if (/adiós|chao|bye/.test(text)) {
    return '¡Hasta luego! Vuelve cuando quieras seguir practicando.';
  }
  return '¡Qué bien! Cuéntame más o hazme una pregunta sobre el idioma.';
}

module.exports = router;
