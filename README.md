# top-charts
Построение рейтинга лучших статей vc.ru: https://t.me/vctopcharts

### Build
```
dotnet publish -c Release -r win-x64
```
### Антирейтинг
```
var from = new Date(2022, 6 - 1, 1).getTime() / 1000;
var to = new Date(2022, 7 - 1, 1).getTime() / 1000;
db.getCollection('Items').find({Site: 1, ExtraData: {$exists: true}, "Data.Date": {$gte: from, $lt: to}})
.sort({ "ExtraData.Dislikes": 1});
```
