const bcrypt = require('bcrypt');
let rPassword = "aabcde";

bcrypt.genSalt(10, function(err, salt) {
    if(err) {
        console.log(err);
    }
    bcrypt.hash(rPassword, salt, (err, hash)=> {
        console.log(hash);
    });
});