const express = require('express');
const keys = require('./config/keys');
const bodyParser = require('body-parser');

const app = express();

app.use(bodyParser.urlencoded({extended: false}));

// Setting up database
const mongoose = require('mongoose');

main().catch(err => console.log(err));

async function main() {
  await mongoose.connect(keys.mongoURI, {useNewUrlParser: true, useUnifiedTopology: true});
}

// Setup database model
require('./model/Account');

// Setup the routes
require('./routes/authenticationRoutes')(app);

require('./wsServer');

app.listen(port=keys.port, ()=>{
    console.log(`Http Server listening on port: ${port}`);
});