const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
    ],
    target: "http://localhost:5252",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
