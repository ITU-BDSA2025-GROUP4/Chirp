# Using the CLI
```
Usage:
      Chirp.CLI interactive [--azure]
      Chirp.CLI read [--azure]
      Chirp.CLI chirp <text> [--azure]
      Chirp.CLI (-h | --help)
      Chirp.CLI --version
```

Please use the flag `--azure` to make the application communicate with the deployed Azure Web app. If the `--azure` flag is not present, the CLI will expect that the API is listening on `localhost` with port `5000`.

To launch the Web API (Chirp.API) please use the following command to ensure it runs on the correct port:
```
dotnet run --project src/Chirp.API --urls=http://localhost:5000/
```
