Travel Planner

1.Tema:
	Aplikacija za planiranje putovanja

2.Arhitektura sistema:
	3 mikroservisa:
		UserService - Autentifikacija i upravljanje korisnicima
		TravelService - Upravljanje planovima, destinacijama, aktivnostima...
		ExpenseService - Upravljanje troskovima i deljenje planova

3. Tehnologije:
	Backend:
		ASP.NET Core 8.0
		Microsoft Service Fabric
		Entity Framework Core
		Microsoft SQL Server
		JWT autentifikacija
		BCrypt hesiranje lozinki
	Frontend:
		React 19
		Axios
		Context API

4. Pokretanje:
	Backend:
		1) Otvaranje TravelPlanner.sln u visual studio 2022
		2) Pokrenuti migracije za svaki od 3 servisa
			cd UserService
			dotnet ef database update
		3)Pokrenuti solution F5
	Frontend:
		1) http://localhost:3000
		2) otvaranjem konzole u vs code, shift + ~,i komanda npm start

5. Korisnicke uloge:
	user - moze kreirati i upravljati svojim planovima putovanja
	admin - isto sto i user, samo moze da upravlja korisnickim nalozima

6. Funkcionalni zahtevi:
	-  Upravljanje planovima putovanja (CRUD)
	-  Upravljanje destinacijama
	-  Organizacija aktivnosti po danima (calendar view)
	-  Evidencija troškova i budžeta
	-  Checklist / packing lista
	-  Registracija, logovanje i korisničke uloge (user/admin)
	-  Deljenje plana putem QR koda (VIEW/EDIT pristup)
	-  Admin panel za upravljanje korisnicima

7. Napomene:
	URL-ovi se citaju iz .env fajla na frontend strani
	JWT token vazi 7 dana
	Lozinke su hesiranje BCrypto algoritmom
	Brisanje plana putovanja automatski brise sve povezane entitete
