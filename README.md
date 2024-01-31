# orderbook-api-dotnet-ng

გასასტარტად: docker-compose -f docker_compose.yml up --build (ცოტა დიდხანს შეიძლება დაჭირდეს)


კონტეინერების დასტარტვისას შეიძლება სხვადასხვა მიმდევრობით დაისტარტონ და ერთმანეთზე დამოკიდებულების გამო ზოგი დაფეილდეს, მაგრამ დაფეილებული კონტეინერის დასტარტვა შველის.


სვაგერები:
Wallet Service - localhost:3276/swagger
Order Service - localhost:3275/swagger

სოკეტზე დასაკავშირებლად (postman-ით):
მისამართი: ws://localhost:8787/updates
ჯერ უნდა დავუქონექდეთ და შემდეგ გავუგზავნოთ ეს რექვესტი:
{"protocol":"json","version":1}
და ჰედერებში გავაყოლოთ
userId : Guid, იმ იუზერის გუაიდი რომელზეც გვინდა მოვიდეს აფდეითები როცა Wallet-ზე შეიცვლება რამე.
![image](https://github.com/nikagobe/orderbook-api-dotnet-ng/assets/91562629/3c547431-5e10-4c38-b2fb-44a168b69072)

ორდერ ბუქის აფდეითები მოდის ყოველ 5 წამში.


Endpoints:
Post User -> აბრუნებს ახალ იუზერს და მისი ორი WalletId-ს
Get User -> აბრუნებს იუზერს იუზერნეიმის მიხედვით

Sell/Btc -> აქ OutGoingWalletId-ში უნდა ჩავუწეროთ ბიტკოინის ვოლეტის აიდი, IncomingWalletId-ში უნდა ჩავუწეროთ Usdt ვოლეტის აიდი. Outgoing-დან გავა ბიტკოინები და Incoming-ში დაირიცხება USDT.
Buy/Btc -> აქ პირიქით OutgoingWalletId-ში USDT-ს ვოლეტი, IncomingWalletId-ში BTC-ს აიდი.


