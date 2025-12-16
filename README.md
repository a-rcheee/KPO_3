# KPO_3

 Проект реализует систему, состояющую из микросервисов с использованием ASP.NET Core и контейнеризации Docker, которая позволяет студентам отправлять работы, а преподавателям получать отчёты о плагиате

- **API Gateway** - принимает запросы от пользователей, направляет их в нужные микросервисы и возвращает единый ответ клиенту
- **FileStorageService** - хранение файлов работ, а также предоставление API для загрузки и скачивания файлов
- **FileAnalysisService** - регистрация сдач, проверка плагиата, формирование отчётов

Принципы архитектуры:

1. Чистая архитектура: каждый микросервис разделён на слои Entities, UseCases, Infrastructure и Host

2. Синхронные взаимодействия: API Gateway обращается к FileStorageService и FileAnalysisService через HttpClient для загрузки файла, получение отчётов

3. DTO: обмен данными осуществляется через строго типизированные объекты

4. Контейнеризация: каждый сервис упакован в Docker-контейнер и вся система разворачивается через docker-compose

# Требования

Docker Compose

Свободные порты:

5000 - API Gateway + Swagger

5001 - FileStorageService + Swagger

5002 - FileAnalysisService + Swagger

# Инструкция по запуску

Перейти в папку проекта, где лежит `docker-compose.yml`.

Поднять всю систему: `docker compose up --build`

# Доступные сервисы после запуска

API Gateway (единая точка входа), Swagger UI: `http://localhost:5000/swagger`

FileStorageService (хранение файлов), Swagger UI: `http://localhost:5001/swagger`

FileAnalysisService (проверка и отчёты), Swagger UI: `http://localhost:5002/swagger`

# Swagger и проверка работоспособности

**GET** `/health` - проверка, что API Gateway запущен и отвечает

**POST** `/api/works/{workId}/submissions` - сдача работы студентом

**GET** `/api/works/{workId}/reports` - получение отчётов по заданию workId

**POST** `/api/pending/process` - обработка отложенных задач: используется, если во время сдачи был недоступен сервис анализа

# Сценарий: Сдача работы студентом (загрузка файла)

Студент отправляет работу через API Gateway:

`POST /api/works/{workId}/submissions`
`Content-Type: multipart/form-data`
Параметры:

`workId` (path)

`studentName` (query)

`file` (form-data)

API Gateway выполняет оркестрацию:

отправляет файл в FileStorageService (`POST /files`)

получает storedFile (`fileId`, `sha256`, `originalFileName`)

отправляет запрос на анализ в FileAnalysisService (HTTP через `HttpClient`)

Gateway возвращает результат сдачи

# Сценарий: Получение отчётов преподавателем по заданию

1. Преподаватель запрашивает отчёты по заданию:

`GET /api/works/{workId}/reports`

2. API Gateway проксирует запрос в FileAnalysisService (`GET /works/{workId}/reports`) и возвращает результат:

```
  {
    "id": "a90a26a2-65fd-43c7-a40b-2ebbd75bb7c5",
    "createdAtUtc": "2025-12-15T20:36:55.7302725",
    "isPlagiarism": true,
    "matchId": "ee37222b-e96e-435b-ba01-c848ed1532c0",
    "reason": "Одинаковый хеш файла с более ранней сдачей (ee37222b-e96e-435b-ba01-c848ed1532c0) by IVAN",
    "submission": {
      "id": "f08bc7b1-4b9d-4e31-ac90-6698390439cf",
      "workId": "1",
      "studentName": "PETR",
      "submittedAtUtc": "2025-12-15T20:36:55.6380825",
      "fileId": "cb14eed5a5514749bb7e41edec6014e3",
      "sha256": "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD",
      "originalFileName": "test.txt"
    },
    "wordCloudUrl": "/api/submissions/f08bc7b1-4b9d-4e31-ac90-6698390439cf/wordcloud"
  }
```

# Алгоритм обнаружения плагиата

1. При сдаче работы вычисляется SHA-256 хэш содержимого загруженного файла

2. Для того же задания (`workId`) система ищет более раннюю сдачу другого студента с таким же sha256

3. Если совпадение найдено то отчёт помечается как плагиат:

`isPlagiarism = true`

`matchId` указывает на `submissionId` более ранней сдачи

`reason` содержит пояснение у кого и с чем совпало

4. Если совпадений нет то `isPlagiarism = false`.
