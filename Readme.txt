// ASP.Net Core (WEB API)
// Entity Framework + MsSQL
Если лень все это читать, там есть swagger.

Задание:
Тестовое задание для программистов С#
Создать веб сервис (без пользовательского веб интерфейса) используя ASP.NET реализующее справочник регионов (свойства: идентификатор, наименование, сокращённое наименование), предоставляющее REST-API на чтение и изменение справочника, справочник должен храниться в БД (желательно Oracle).
Преимуществом будет использование кэширования и логирования.
Необходимо предоставить примеры HTTP запросов на сервис (например, PowerShell или в любом другом удобном виде).

Примеры запросов:
пример запроса всех регионов:[GET]
curl -X GET "https://localhost:44324/api/Regions" -H  "accept: text/json"
вернет список всех регионов:
[
  {
    "id": 1,
    "name": "Primorye",
    "fullName": "Primorsky Kray"
  },
  {
    "id": 2,
    "name": "Khabarovsky",
    "fullName": "Khabarovsky Kray"
  }
]

пример запроса конкретного региона по id, например с ID=1:[GET]
curl -X GET "https://localhost:44324/api/Regions/4" -H  "accept: text/plain"
вернет конкретный регион ли NotFound[404] если такого id нет
{
    "id": 1,
    "name": "Primorye",
    "fullName": "Primorsky Kray"
}

пример запроса на создание региона [POST]
curl -X POST "https://localhost:44324/api/Regions" -H  "accept: text/plain" -H  "Content-Type: text/json" -d "{\"id\":0,\"name\":\"Tatarstan\",\"fullName\":\"Republic Tatarstan\"}"
вернет ok[200], или если некорректный запрос BadRequest[400]

пример запроса на обновление региона [PUT]
curl -X PUT "https://localhost:44324/api/Regions/5" -H  "accept: */*" -H  "Content-Type: application/json" -d "{\"id\":3,\"name\":\"Tatarstan\",\"fullName\":\"Republic Tatarstan (changed)\"}"
вернет ok[200], или если некорректный запрос BadRequest[400]

пример запроса на удаление региона [DELETE] 
curl -X DELETE "https://localhost:44324/api/Regions/5" -H  "accept: */*"
вернет ok[200], если такой id есть или NotFound[404] если нет


