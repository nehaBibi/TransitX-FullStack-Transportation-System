<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Student Registration Form</title>

<style>

*{
    margin:0;
    padding:0;
    box-sizing:border-box;
    font-family:Arial;
}

body{
    background:#f2f2f2;
    display:flex;
    justify-content:center;
    align-items:center;
    height:100vh;
}

.container{

    width:400px;
    background:white;
    padding:20px;
    border-radius:10px;
    box-shadow:0px 0px 10px gray;

    display:flex;
    flex-direction:column;
    gap:15px;
}

h2{
    text-align:center;
}

input{

    padding:10px;
    font-size:16px;
}

button{

    padding:10px;
    background:blue;
    color:white;
    border:none;
    cursor:pointer;
    font-size:16px;
}

button:hover{

    background:darkblue;
}

#message{

    text-align:center;
    font-weight:bold;
    margin-top:10px;
}

@media(max-width:500px){

.container{

width:90%;

}

}

</style>

</head>

<body>

<div class="container">

<h2>Student Registration Form</h2>

<form id="myForm">

<input type="text" id="name" placeholder="Enter Name">

<input type="email" id="email" placeholder="Enter Email">

<input type="password" id="password" placeholder="Enter Password">

<button type="submit">Register</button>

</form>

<p id="message"></p>

</div>

<script>

const form=document.getElementById("myForm");

form.addEventListener("submit",function(e){

e.preventDefault();

let name=document.getElementById("name").value;
let email=document.getElementById("email").value;
let password=document.getElementById("password").value;

if(name==""){

alert("Name is required");
return;

}

if(email==""){

alert("Email is required");
return;

}

if(password.length<6){

alert("Password must be at least 6 characters");
return;

}

document.getElementById("message").innerHTML="Registration Successful";

document.getElementById("message").style.color="green";

fetch("https://jsonplaceholder.typicode.com/posts",{

method:"POST",

headers:{

"Content-Type":"application/json"

},

body:JSON.stringify({

name:name,
email:email,
password:password

})

})

.then(response=>response.json())

.then(data=>{

console.log(data);

})

.catch(error=>{

console.log(error);

});

});

</script>

</body>
</html>