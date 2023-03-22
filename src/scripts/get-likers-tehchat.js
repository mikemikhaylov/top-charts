//should be extracted from url, eg https://tenchat.ru/media/1121351-issledovaniye-rynka-byutitovarov-v-ecommerce
const publication_id = 1121351;
fetch(`https://tenchat.ru/gostinder/api/web/auth/post/id/${publication_id}/liked/user?page=0&size=20000`)
    .then((response) => response.json())
    .then((data) => {
        let result = '';
        data.forEach(user => {
            const username = user.username ?? user.defaultUsername;
            const name = (user.name + ' ' + user.surname).trim();
            result +=`${user.id};${username};${name};https://tenchat.ru/${username}\n`
        });
        console.log(result);
    });
