import { expect, test, type Page } from "@playwright/test";
import { existsSync, writeFileSync } from "node:fs";

const base = process.env.BASE_URL || "http://localhost:8088";
const password = process.env.DEV_USER_PASSWORD || "lab-dev-user";
const flag = process.env.OPS_FLAG;
const ready = process.env.OPS_READY;

test("el frontend sigue operando durante k6 y la resiliencia", async ({ page }) => {
  test.setTimeout(20 * 60 * 1000);
  if (!flag || !ready) {
    throw new Error("OPS_FLAG y OPS_READY tienen que venir de Invoke-Tests.ps1.");
  }

  page.setDefaultTimeout(30_000);
  page.setDefaultNavigationTimeout(30_000);
  writeFileSync(ready, "listo");

  let completed = 0;
  while (existsSync(flag)) {
    try {
      await createFromTheFrontend(page);
      completed += 1;
      console.log(`operacion ok ${completed}`);
    } catch (error) {
      const message = error instanceof Error ? error.message.split("\n")[0] : String(error);
      console.log(`operacion interrumpida: ${message}`);
    }
  }

  expect(completed, "el frontend no completó ninguna operación mientras corrían k6 y la resiliencia").toBeGreaterThan(0);

  await expect
    .poll(
      async () => {
        try {
          await createFromTheFrontend(page);
          return true;
        } catch {
          return false;
        }
      },
      { timeout: 90_000, intervals: [2_000] },
    )
    .toBe(true);
});

async function createFromTheFrontend(page: Page) {
  const title = `Operacion ${Date.now()}`;
  await page.goto(`${base}/login`);
  await page.getByLabel("Usuario").fill("dev-user");
  await page.getByLabel("Contraseña").fill(password);
  await page.getByRole("button", { name: "Entrar" }).click();
  await expect(page.getByRole("heading", { name: "Tareas" })).toBeVisible();
  await page.getByRole("link", { name: "Nueva tarea" }).click();
  await page.getByLabel("Título").fill(title);
  await page.getByLabel("Descripción").fill("Operación durante la prueba de infraestructura");
  const created = page.waitForResponse(
    (response) => response.url().includes("/api/v1/tasks") && response.request().method() === "POST",
  );
  await page.getByRole("button", { name: "Guardar" }).click();
  const response = await created;
  if (response.status() !== 201) {
    throw new Error(`crear respondió ${response.status()}`);
  }
  await expect(page.getByRole("heading", { name: title })).toBeVisible();
}
