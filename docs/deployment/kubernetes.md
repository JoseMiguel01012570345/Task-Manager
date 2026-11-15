# Despliegue

Los manifiestos de `infrastructure/k8s` crean el namespace `taskmanagement`, la API y el frontend. Cada uno arranca con dos réplicas para que una caída no deje el servicio sin proceso. El scheduler intenta colocarlas en nodos distintos y el presupuesto de disrupción exige que quede al menos una disponible. El autoscaler de la API mantiene entre 2 y 5 réplicas según el uso de CPU. Los Service son ClusterIP: la API no publica un puerto del nodo. El Ingress usa la clase `traefik` y es la entrada del clúster.

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
