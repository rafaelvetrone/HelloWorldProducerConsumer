Para executar o programa faça o seguinte:
- faça "docker pull" das imagens do wurstmeister/kafka e do wurstmeister/zookeeper.
- execute no diretório do projeto docker-compose up (vai subir o kafka no docker).
- Compile o projeto.
- Copie os binários em 2 pastas diferentes.
- Abra o appsettings nas 2 instâncias e certifique-se de que as portas no endereço da url estejam diferentes senão vai dar conflito.
- execute com "dotnet HelloWorldProducerConsumer.dll" nas 2 instâncias.