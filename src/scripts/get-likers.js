
const publication_id = 604957;//replace with your publication id
fetch(`https://vc.ru/vote/get_likers?id=${publication_id}&type=1&mode=raw`)
    .then((response) => response.json())
    .then((data) => {
        const likers = data.data.likers;
        let result = '';
        Object.keys(likers)
            .sort()
            .filter(key => likers[key].sign > 0)
            .forEach(key => result +=`${key};${likers[key].user_name};${likers[key].user_url}\n`);
        console.log(result);
    });
