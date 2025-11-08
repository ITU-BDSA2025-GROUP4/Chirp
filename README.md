# Chirp!

---

## Welcome to Chirp!

![chirp banner](README-resources\Chirp.png)

`Chirp!` is a lightweight social media-like platform for sending and receiving `Cheeps`, Short messages for other Chirp users to read and respond to.

## INSTALLATION
### Prerequisites

- `dotnet 8.0` installed
- `Git` installed

### Clone the repository
Clone the project from the `github.com` repository, or by running bash command from your prefered location:  

`bash: git clone https://github.com/ITU-BDSA2025-GROUP4/Chirp`

If necessary, install required dependencies. 

### Launch

For launching `Chirp!`, navigate to `Chirp/src/Chirp.Web` and run

`bash: dotnet run`

or run the subdirectory as an argument from the main directory

`bash: dotnet run --project src/Chirp.Web/`

If done correctly, this will open a localhost at port `localhost:5273` . Copy the localhost and paste it in your browser of choice.

## USAGE

Having launched Chirp! you should now be able to navigate the UI from your browser. You will first be met with the `Public Timeline`, showing all posts sorted by recensy. Here you can either `register` as a new user, or `login` with an existing profile.

![Public timeline with register and login buttons](README-resources\publicTimeline.png)

### Register

To register, fill out the provided form on the `Register` page.

![Register form](README-resources\Register.png)

### Login

To login, fill in the credentials from your existing account into the `login` form.

![](README-resources\Login.png)

If done correctly, you should be logged in to your account.

### Post a Cheep!

Now you are free to post anything on your mind for the world to see!

![](README-resources\Post.png)

Remember, you can log out at any time. Feel free to navigate the many posts on `Chirp!`. And remember, by clicking on a `username`, you can see that person's post `Timeline`.


## AUTHORS AND CONTRIBUTORS

Authors of this project are

-  `augustlh`
-  `KumaSC`
-  `Mojjedrengen`
-  `RockRottenSalad`
-  `V0idshock`

With further contributions from

- `oshelITU`

and special thanks to the `ITU BDSA course team` 
## LICENSE
This program uses the MIT License - For further reading consult [LICENSE.md](LICENSE.md)