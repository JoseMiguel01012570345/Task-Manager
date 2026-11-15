import http from "k6/http";
import { check, sleep } from "k6";

const base = __ENV.BASE_URL || "http://localhost:8088";
const suite = __ENV.SUITE === "1";

export const options = {
  gracefulStop: "1s",
  gracefulRampDown: "1s",
  stages: suite
    ? [
        { duration: "2s", target: 2 },
        { duration: "3s", target: 8 },
        { duration: "3s", target: 2 },
        { duration: "2s", target: 0 },
      ]
    : [
        { duration: "10s", target: 5 },
        { duration: "10s", target: 80 },
        { duration: "20s", target: 5 },
        { duration: "10s", target: 0 },
      ],
};

export default function () {
  const response = http.get(`${base}/health/live`);
  check(response, { "answered": (result) => result.status === 200 || result.status === 429 });
  sleep(0.1);
}
