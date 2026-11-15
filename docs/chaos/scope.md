# Chaos Mesh

Los experimentos viven en `infrastructure/k8s/chaos` y solo seleccionan pods del namespace `taskmanagement`. No hay un experimento que abarque el clúster entero.

La instalación prevista, cuando exista el clúster k3d, es el chart de Chaos Mesh en ese mismo namespace. En esta máquina k3d no está instalado, así que el controlador no se desplegó y ningún experimento llegó a ejecutarse. Los informes quedan inconclusos a propósito: no se inventan tiempos de recuperación.
