UserManagement


Get User (POST)  (email or username)
https://livestoriesusermanagement.azurewebsites.net/api/FWDUser/username
{
    "username": "exampleUsername",
    "email": "email"
}


Sign Up (POST)
https://livestoriesusermanagement.azurewebsites.net/api/FWDUser/signup
{
    "UserName": "thename",
    "FirstName": "firsname",
    "LastName": "lastname",
    "MiddleName": "m",
    "Phone": "425-123-5555",
    "Email": "theemail",
    "Secret": "thesecret"
}


Sign On (POST)  (email or username)
https://livestoriesusermanagement.azurewebsites.net/api/FWDUser/signon
{
    "username": "exampleUsername",
    "email": "email",
    "secret": "exampleSecret"
}


Update User (PUT)
https://livestoriesusermanagement.azurewebsites.net/api/FWDUser/updateuser
{
    
}


Delete User (DELETE)
https://livestoriesusermanagement.azurewebsites.net/api/FWDUser/deleteuser
{
    "username": "exampleUsername",
    "email": "exampleemail"
}


Change Password (POST)  (email or username)
https://livestoriesusermanagement.azurewebsites.net/api/FWDUser/changepw
{
    "username": "exampleUsername",
    "email": "exampleUsername",
    "currentsecret": "oldPassword",
    "secret": "newPassword"
}



Communcation Management


Get Items by User (POST)
https://livestoriescommservice.azurewebsites.net/api/CommunicationItems/getitemsbyuser
{
    "username": "exampleUsername"
}


Get Items by Phone (POST)
https://livestoriescommservice.azurewebsites.net/api/CommunicationItems/getitemsbyphone
{
    "phone": "examplePhone"
}



Get Items by Contact List (POST)
https://livestoriescommservice.azurewebsites.net/api/CommunicationItems/getitemsbycontactlist
{
    "contacts": [
        "contact": "email1",
        "contact": "email2",
        "contact": "phone1",
        "contact": "phone2",
        "contact": "phone3"
    ]
}



Get Items by Email (POST)
https://livestoriescommservice.azurewebsites.net/api/CommunicationItems/getitemsbyemail
{
    "email": "exampleEmail"
}



Get Items by Application ID (POST) 
https://livestoriescommservice.azurewebsites.net/api/CommunicationItems/getitemsbyapplicationid
{
    "application": "exampleApp"
}



Add Entry (POST) add communication entry
https://livestoriescommservice.azurewebsites.net/api/CommunicationItems/addentry
{
    
}