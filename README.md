# Systemctl для LinCtrl API

Плагин для Linux с systemd. Маршруты `GET /systemctl/services` и `GET /systemctl/services/{name}` доступны роли наблюдателя и выше. Запуск, остановка и перезапуск доступны администратору через `POST /systemctl/services/{name}/{start|stop|restart}`.

Службы для управления задаются в конфигурации хоста:

```json
{
  "Systemctl": {
    "AllowedUnits": ["cron.service", "docker.service"]
  }
}
```

Администратор может создать службу из файла, изменить или удалить локальный файл службы в `/etc/systemd/system`. Имя новой службы тоже должно быть в `AllowedUnits`. Файлы пакетов и символические ссылки редактировать и удалять через API нельзя.

Авторестарт настраивается отдельно от автозапуска через `Restart=on-failure` или `Restart=no`. Плагин сохраняет правило в drop-in службы. Добавление в target создаёт постоянную зависимость `Wants=` от выбранного target к службе. Изменение файла и добавление в target не запускают службу.

Для Raspberry Pi с 64-битной ОС используйте `dotnet publish SystemctlPlugin/SystemctlPlugin.csproj -c Release -r linux-arm64` на Linux ARM64. Результат публикации помещается в `plugins/systemctl` рядом с API.
