# Sistemas de Redes para jogos : Jogo de ação em Unity  
## Trabalho Realizado por:  
- David Mendes 22203255
## Link para o repositório:  
https://github.com/ArKynn/UnityNetworkProject  
## Relatório: 

Comecei este projeto logo a criar a base do projeto no Unity. Eu já sabia ao inicio que queria fazer um jogo de ação no Unity sem login/matchmaking, que foi o que eu fiz para este projeto. A minha ideia a vir para este projeto seria um jogo 2D PvP DeathMatch com melee combat.  

Utilizei o seguinte asset pack: https://assetstore.unity.com/packages/2d/characters/hero-knight-pixel-art-165188, para criar o personagem e o seguinte asset pack: https://assetstore.unity.com/packages/2d/environments/pixel-art-top-down-basic-187605, para criar o mundo do jogo.  

Após criar o mundo de jogo, fiz o animator para o personagem e criei um script base para o Player, ainda sem contar com qualquer elemento relacionado ao Unity Netcode for GameObjects, para ter uma base por onde me guiar.  
Este script já inclui a detecção de input, movimento do player, acionamento do animator, vida e morte do personagem, ataques, detecção de colisões no ataque e dano infligido a outro jogador.  

Depois de ter uma base funcional, fui assistir ao seguinte video: https://www.youtube.com/watch?v=swIM2z6Foxk, para me informar sobre o Unity Netcode for GameObjects.  

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

Sem mudar nada, o personagem imóvel não respondia aos ataques.

## Bibliografia:  
https://www.youtube.com/watch?v=3yuBOB3VrCk  
https://www.youtube.com/watch?v=swIM2z6Foxk  
https://assetstore.unity.com/packages/2d/characters/hero-knight-pixel-art-165188  
https://assetstore.unity.com/packages/2d/environments/pixel-art-top-down-basic-187605  
