import { expect, test } from "@playwright/test";
import { readdirSync, readFileSync, statSync } from "node:fs";
import path from "node:path";

const payload = `<img src=x onerror="window.__xss=true"><script>window.__xss=true</script>`;

test("stored text stays text", async ({ page }) => {
  await page.setContent(`<ul><li id="out"></li></ul>`);
  await page.evaluate((value) => {
    document.getElementById("out")!.textContent = value;
  }, payload);
  await expect(page.locator("#out")).toHaveText(payload);
  expect(await page.evaluate(() => (window as unknown as { __xss?: boolean }).__xss ?? false)).toBe(false);
});

test("reflected query stays text", async ({ page }) => {
  await page.setContent(`<div id="out"></div>`);
  await page.evaluate((value) => {
    const params = new URLSearchParams(`q=${encodeURIComponent(value)}`);
    document.getElementById("out")!.textContent = params.get("q");
  }, payload);
  await expect(page.locator("#out")).toHaveText(payload);
  expect(await page.evaluate(() => (window as unknown as { __xss?: boolean }).__xss ?? false)).toBe(false);
});

test("dom hash stays text", async ({ page }) => {
  await page.setContent(`<div id="out"></div><script>
    window.addEventListener("hashchange", () => {
      document.getElementById("out").textContent = decodeURIComponent(location.hash.slice(1));
    });
  </script>`);
  await page.evaluate((value) => {
    location.hash = encodeURIComponent(value);
  }, payload);
  await expect(page.locator("#out")).toHaveText(payload);
});

test("the react source does not inject html", () => {
  const root = path.resolve(__dirname, "../../src/TaskManagement.Web/src");
  for (const file of filesUnder(root)) {
    const source = readFileSync(file, "utf8");
    expect(source).not.toContain("dangerouslySetInnerHTML");
    expect(source).not.toContain("innerHTML");
  }
});

function filesUnder(directory: string): string[] {
  return readdirSync(directory).flatMap((name) => {
    const full = path.join(directory, name);
    return statSync(full).isDirectory() ? filesUnder(full) : [full];
  });
}
