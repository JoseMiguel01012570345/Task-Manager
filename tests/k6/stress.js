import http from "k6/http";
import { check, sleep } from "k6";

const base = __ENV.BASE_URL || "http://localhost:8088";
const suite = __ENV.SUITE === "1";

export const options = {
  gracefulStop: "1s",
  gracefulRampDown: "1s",
  stages: suite
    ? [
        { duration: "3s", target: 4 },
        { duration: "5s", target: 8 },
        { duration: "3s", target: 0 },
      ]
    : [
        { duration: "30s", target: 10 },
        { duration: "1m", target: 40 },
        { duration: "30s", target: 0 },
      ],
};

export default function () {
  const response = http.get(`${base}/health/live`);
  check(response, { "answered": (result) => result.status === 200 || result.status === 429 });
  sleep(0.2);
}
