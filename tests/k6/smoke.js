import http from "k6/http";
import { check, sleep } from "k6";

const base = __ENV.BASE_URL || "http://localhost:8088";
const suite = __ENV.SUITE === "1";

export const options = {
  vus: 1,
  duration: suite ? "5s" : "30s",
  gracefulStop: "1s",
  thresholds: {
    http_req_failed: ["rate<0.05"],
  },
};

export default function () {
  const response = http.get(`${base}/health/live`);
  check(response, { "live responds 200": (result) => result.status === 200 });
  sleep(1);
}
