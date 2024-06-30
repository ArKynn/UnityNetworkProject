# Sistemas de Redes para jogos : Jogo de ação em Unity  
## Trabalho Realizado por:  
- David Mendes 22203255
## Link para o repositório:  
https://github.com/ArKynn/UnityNetworkProject  
## Diagrama Arquitetura Redes  
  
![DiagramaRedes(1)](https://github.com/ArKynn/UnityNetworkProject/assets/115217596/173c7d1a-d2ff-4b0e-b382-2851b5a12be3)  
  
## Relatório: 

Comecei este projeto logo a criar a base do projeto no Unity. Eu já sabia ao inicio que queria fazer um jogo de ação no Unity sem login/matchmaking, que foi o que eu fiz para este projeto. A minha ideia a vir para este projeto seria um jogo 2D PvP DeathMatch com melee combat.  

Utilizei o seguinte asset pack: https://assetstore.unity.com/packages/2d/characters/hero-knight-pixel-art-165188, para criar o personagem e o seguinte asset pack: https://assetstore.unity.com/packages/2d/environments/pixel-art-top-down-basic-187605, para criar o mundo do jogo.  

Após criar o mundo de jogo, fiz o animator para o personagem e criei um script base para o Player, ainda sem contar com qualquer elemento relacionado ao Unity Netcode for GameObjects, para ter uma base por onde me guiar.  
Este script já inclui a detecção de input, movimento do player, acionamento do animator, vida e morte do personagem, ataques, detecção de colisões no ataque e dano infligido a outro jogador.  

Depois de ter uma base funcional, fui assistindo ao seguinte video: https://www.youtube.com/watch?v=swIM2z6Foxk, para me informar sobre o Unity Netcode for GameObjects e abri o projeto MPWyzards para a ajuda na resolução de problemas.  

Enquanto assitia, importei o Unity Netcode for GameObjects para o projeto, adicionei o Network Manager e o Unity Transport a um objeto separado, adicionei o component Network Object ao player e tornei-o um player prefab.  

Iniciando o play mode do Unity Editor, e selecionando a opção "Start Host" do Network Manager, o personagem corretamente aparecia no mundo.  
  
![Screenshot_1](https://github.com/ArKynn/UnityNetworkProject/assets/115217596/4b66d6c5-75ec-4828-b73d-85ccba8b11f9)  
  
Importei o Asset ParrelSync, para poder fazer testes dentro do editor. Abri uma instância como "Start Server" e outra como "Start Client". O personagem aparecia no mundo, eu conseguia-o mexer, mas o cliente e o servidor não estavam sincronizados.  

Para resolver este problema, adicionei o behaviour "Network Transform" ao jogador, fiz com que o personagem apenas recebia inputs do cliente a que lhe corresponde e testei de novo.  

Desta vez, nem o cliente, nem o servidor registavam mudanças no personagem, ele apenas ficava parado. Após procurar uma solução, percebi que o behaviour Network Transform que adicionei ao jogador sincroniza o transform do player do servidor com os clientes e não ao contrário, ou seja, eu precisava de adicionar uma forma com que seja o servidor a mexer o personagem e não o cliente.  

Pensando qual abordagem seria a melhor, optei por tornar o movimento do personagem autoritário do servidor. Troquei o metodo do movimento para usar o ServerRpc e testei.  
  
![Screenshot_3](https://github.com/ArKynn/UnityNetworkProject/assets/115217596/275c004b-f826-4a1b-a7e0-a82e9904b050)  
  
O jogador já se mexia, mas quando tentava atacar, nada acontecia. O esperado seria o personagem parar momentáriamente, atacar e depois voltar a correr. Testei e tentei resolver este problema mas continuava ou a não atacar ou quando atacava no servidor, não o fazia no cliente.  

Com estes problemas, decidi tornar o movimento e o ataque autoritário do cliente. Troquei para o Client Network Transform e testei. 
  
![Screenshot_2](https://github.com/ArKynn/UnityNetworkProject/assets/115217596/30860145-5db0-41a6-9f32-35119ea4836d)  
  
Agora o cliente andava e atacava, mas o servidor só andava. Fui testando e pesquisando sobre o problema e verifiquei que o erro era a falta de um Network Animator.  

Adicionei-o e testei. Igual, no servidor, o personagem continuava sem atacar. Fiz com que o metodo que aciona as animações do ataque fosse computado no servidor.  

Desta vez, os dois atacavam, mas a animação parecia repetir o inicio várias vezes. Fui investigar e reparei que a variavel que controla quando o jogador pode atacar era gerida no metodo dos ataques, ou seja, enquanto tinha o botão do ataque premido, o personagem iria começar um ataque repetidamente.  

Fui testando e iterando até perceber que existe uma variante do Network Animator chamado Client Network Animator. Com os problemas que estava a ter e com os possiveis problemas que eu pensei que iria ter com a deteção de colisões, decidi tornar as animações autoritárias do cliente e adicionei esse behaviour.  

Desta vez, a animação funcionava bem dos dois lados, e eu continuava a conseguir controlar quando o personagem pode ou não atacar. 

Com as animações funcionais, adicionei um personagem imóvel ao projeto para testar a deteção de colisões.  

Sem mudar nada, o personagem imóvel não respondia aos ataques. Ao procurar o problema, percebi que o jogador colidia com o personagem imovel, mas a informação de que foi atacado só acontecia no cliente.  

Troquei a forma como informava da colisão para ser pelo servidor, através do uso de Server Rpc e testei.  
  
![Screenshot_4](https://github.com/ArKynn/UnityNetworkProject/assets/115217596/d35271c6-edc9-4906-beca-34afa7961feb)  

A transmição do dano causado não estava a acontecer e resolvi, informando o servidor do dano causado e o servidor atualizado a vida do jogador, que tornei uma Network Variable, para ser automáticamente sincronizada.  
  
![Screenshot_5](https://github.com/ArKynn/UnityNetworkProject/assets/115217596/cf1d0ed9-72ac-4861-9965-01600c483946)  
  
O dano agora era corretamente tensmitido, mas não a sua quantidade. Quando testei com realmente dois cliente, em vez de um personagem imóvel, o jogador atacado registava que recebeu dano mas a vida não descia.  
  
Após a correção de uns erros na transmição do valor do dano enviado, agora os personagens, recebiam dano e morriam corretamente.  
  
![image](https://github.com/ArKynn/UnityNetworkProject/assets/115217596/741c1713-4ed8-4b73-9253-f60e55718384)  

## Conclusões  

Este projeto ajudou bastante a experienciar a base o que é programar um jogo multiplayer, visto que tive de lidar com diversos problemas relacionados com a comunicação entre o cliente e o servidor. Apesar disso, não foi algo que me diretamente ensinou mais sobre tudo o que está por detrás do Unity Netcode for GameObjects e do Unity transport, nem sobre redes e conecção via internet. Ainda assim, acho que foi uma experiencia positiva pois deixou-me mergulhar os pés no vasto oceano que é a internet e os vários sistemas, protocolos e convenções por detrás.  
  
## Instruções  

## Agradecimentos
Agradeço ao professor Diogo Andrade, que me disponibilizou o projeto de sua autoria MPWyzards, que me ajudou a resolver problemas ao comparar a sua implementação do Network for GameObjects com a deste projeto.  
## Bibliografia  
https://www.youtube.com/watch?v=3yuBOB3VrCk  
https://www.youtube.com/watch?v=swIM2z6Foxk  
https://assetstore.unity.com/packages/2d/characters/hero-knight-pixel-art-165188  
https://assetstore.unity.com/packages/2d/environments/pixel-art-top-down-basic-187605  
