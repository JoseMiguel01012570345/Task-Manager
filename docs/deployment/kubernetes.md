# Despliegue

Los manifiestos de `infrastructure/k8s` crean el namespace `taskmanagement`, la API y el frontend. Cada uno arranca con tres réplicas, repartidas por nodo, y el presupuesto de disrupción exige que quede al menos una disponible. El autoscaler de la API está fijo en 3 (`minReplicas` y `maxReplicas`). Los Service son ClusterIP. El Ingress usa la clase `traefik` y es la entrada de dentro del clúster; el puerto que se abre en el host es el del Traefik del borde, `8088`.

Las imágenes locales se importan antes de aplicar los manifiestos:

```powershell
k3d image import taskmanagement-api:local taskmanagement-web:local -c taskmanagement
kubectl apply -f infrastructure/k8s/namespace.yaml
kubectl apply -f infrastructure/k8s/api.yaml
kubectl apply -f infrastructure/k8s/web.yaml
kubectl apply -f infrastructure/k8s/ingress.yaml
```

La contraseña de la base en el Secret es la del laboratorio, la misma que ya está en Compose.

Las migraciones corren en el Job `migrate` (`dotnet TaskManagement.API.dll --migrate`). Las réplicas no migran al arrancar. El Job se aplica después del Secret y antes de dar por listo el despliegue.
