const mongoose = require('mongoose');
const Account = mongoose.model('accounts');
const bcrypt = require('bcrypt');

module.exports = app => {
    // Routes
    app.post('/account/login', async (req, res) => {

        response = {};

        const { rUsername, rPassword } = req.body;
        if(rUsername == null || rPassword == null)
        {
            response.code = 1;
            response.msg = "Invalid credentials";
            res.send(response);
            return;
        }

        var userAccount = await Account.findOne({ username: rUsername}, 'username password');
        if(userAccount != null){

            bcrypt.compare(rPassword, userAccount.password, async (err, result)=>{
                if(result == true) {

                    userAccount.lastAuthentication = Date.now();
                    await userAccount.save();
                    console.log("Retrieving account...");

                    response.code = 0;
                    response.msg = "Account Found";
                    response.data = (({username}) => ({username}))(userAccount);

                    res.send(response);
                    return;

                } else {

                    response.code = 1;
                    response.msg = "Invalid credentials";
                    res.send(response);
                    return;

                }
            });
            
        } else {
            response.code = 1;
            response.msg = "Invalid credentials";
            res.send(response);
            return;
        }

        
    });

    app.post('/account/create', async (req, res) => {

        response = {};

        const { rUsername, rPassword } = req.body;
        console.log()
        if(rUsername == null || rPassword == null)
        {
            response.code = 1;
            response.msg = "Invalid credentials";
            res.send(response);
            return;
        }

        var userAccount = await Account.findOne({ username: rUsername});
        if(userAccount == null){

            // Create a new account
            console.log("Creating new account...");

            // Generate a unique access token

            bcrypt.genSalt(10, function(err, salt) {
                if(err) {
                    console.log(err);
                }
                bcrypt.hash(rPassword, salt, async (err, hash)=> {
                    var newAccount = new Account({
                        username : rUsername,
                        password : hash,
                        salt : salt,
                        lastAuthentication : Date.now()
                    });
                    await newAccount.save();

                    response.code = 0;
                    response.msg = "Account found";
                    response.data = (({username}) => ({username}))(newAccount);
                    res.send(response);
                    return;
                });
            });

        } else {

            response.code = 2;
            response.msg = "Username is already taken";
            res.send(response);
            return;
        }

    });
}
