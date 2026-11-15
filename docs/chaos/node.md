# Nodo

Chaos Mesh no apaga un nodo de k3d por sí solo de forma portátil. El experimento de nodo es detener un agente:

```powershell
docker stop k3d-taskmanagement-agent-0
docker start k3d-taskmanagement-agent-0
```

El Deployment pide repartir las réplicas y el presupuesto de disrupción deja al menos una. Sin el clúster, este paso no se ejecutó.
