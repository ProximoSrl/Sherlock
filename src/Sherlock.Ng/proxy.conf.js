const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/Temp",
      "/Upload",
      "/status"
    ],
    target: "http://localhost:4002/",
    secure: false,
    logLevel: 'debug',
    ws: true
  }
];

module.exports = PROXY_CONFIG;
