[readme.md](https://github.com/user-attachments/files/33927944/readme.md)
# Prova Tècnica: Gestor de Campanyes de Màrqueting

Benvingut/da a la prova tècnica! L'objectiu d'aquest exercici és avaluar la teva manera de dissenyar codi, estructurar una aplicació, aplicar regles de negoci i testejar la solució.

La prova ha de ser amb Angular i .Net 

⏱ **Temps estimat:** 5 hores (valorem més la qualitat i el disseny del codi que no pas que estigui 100% acabat amb funcionalitats extra).

---

## 1. Context del problema

L'equip de màrqueting necessita una API REST per crear, activar i monitoritzar campanyes publicitàries. Cada campanya té un pressupost màxim i un període de validesa. A mesura que la campanya genera interaccions (clics o conversions), el pressupost es va consumint fins que s'esgota o la campanya caduca.

---

## 2. Requisits Funcionals

### Model de Dades (`Campanya`)
Cada campanya ha de contenir, com a mínim, les següents propietats:
* `id`: Identificador únic (UUID o similar).
* `nom`: Text obligatori (mínim 3 caràcters).
* `pressupostTotal`: Import decimal estrictament positiu ($> 0$).
* `pressupostConsumit`: Import decimal (inicialment $0$).
* `dataInici`: Data d'inici de vigència (format ISO 8601).
* `dataFi`: Data de finalització de vigència (format ISO 8601).
* `estat`: Pot ser `ESBORRANY`, `ACTIVA`, `PAUSADA` o `FINALITZADA`.

### Operacions de l'API

#### 1. Crear una campanya (`POST /campaigns`)
Crea una nova campanya al sistema.
* **Regles de negoci:**
  * Es crea per defecte en estat `ESBORRANY` i amb `pressupostConsumit = 0`.
  * La `dataFi` ha de ser estrictament posterior a la `dataInici`.
  * El `pressupostTotal` ha de ser més gran que $0$.

*Exemple de Payload:*
```json
{
  "nom": "Campanya Estiu 2026",
  "pressupostTotal": 1500.00,
  "dataInici": "2026-06-01T00:00:00Z",
  "dataFi": "2026-08-31T23:59:59Z"
}
```

#### 2. Canviar l'estat d'una campanya (`PATCH /campaigns/{id}/status`)
Permet activar o pausar una campanya existent.
* **Regles de negoci:**
  * Només permet transicionar manualment als estats `ACTIVA` o `PAUSADA`.
  * No es pot activar una campanya si la `dataFi` ja ha passat o si `pressupostConsumit >= pressupostTotal`.
  * Una campanya en estat `FINALITZADA` no pot tornar a canviar d'estat sota cap concepte.

*Exemple de Payload:*
```json
{
  "estat": "ACTIVA"
}
```

#### 3. Registrar consum de pressupost (`POST /campaigns/{id}/consume`)
Rep un import (ex. el cost d'un clic publicitari) i l'acumula al `pressupostConsumit`.
* **Regles de negoci:**
  * L'import a consumir ha de ser estrictament positiu ($> 0$).
  * Només es pot consumir pressupost si la campanya està `ACTIVA` i la data actual es troba entre `dataInici` i `dataFi`.
  * El consum no pot fer que el `pressupostConsumit` superi el `pressupostTotal` (en cas que el superi, cal rebutjar l'operació amb un error adequat).
  * Si amb aquest consum el `pressupostConsumit` iguala el `pressupostTotal`, l'estat de la campanya ha de passar automàticament a `FINALITZADA`.

*Exemple de Payload:*
```json
{
  "import": 25.50
}
```

#### 4. Llistar campanyes (`GET /campaigns`)
Retorna el llistat de campanyes.
* **Requisit:** Ha de permetre filtrar opcionalment per estat mitjançant query params (ex. `GET /campaigns?estat=ACTIVA`).

---

## 3. Requisits Tècnics

* **Llenguatge i Framework:** Pots utilitzar l'stack tecnològic amb què et sentis més còmode/a.
* **Persistència:** Per agilitzar el desenvolupament i no perdre temps amb infraestructura, pots utilitzar una base de dades en memòria (SQLite, H2, etc.) o un repositori en memòria (guardant les dades en estructures de dades en temps d'execució).
* **Testing:** Inclou els tests unitaris i/o d'integració que consideris necessaris per garantir que les regles de negoci es compleixen.
* **Codi net:** Es valorarà l'estructura del projecte, la llegibilitat del codi, el tractament d'errors (codis d'estat HTTP adequats) i un bon historial de commits a Git.

---

## 4. Lliurament i Documentació del Candidat/a

Un cop finalitzada la prova, omple la secció següent en aquest mateix fitxer `README.md` i envia'ns l'enllaç al teu repositori (o un fitxer comprimit que inclogui la carpeta `.git`).

### 🛠 Com executar el projecte

Dins el projecte he posat separat en carpetas la part de backend i la part de frontend. 
Dins de backend obrir la solució, restaurar dependencies nuget per si cal amb ``dotnet restore`` i iniciar el projecte en local. 
En la carpeta frontend obrir el projecte, instal·lar dependencies ``npm install`` i arrencar l'aplicacio amb ``npm start``. Aquest projecte consumeix l'api en local amb la ruta corresponent.

### 🧪 Com executar els tests
En el projecte api desde el explorador de tests es pot executar indivisualment o grupalment els tests realitzats amb xunit.

### 🏛 Decisions d'arquitectura i disseny

Backend .NET
El projecte ha estat organitzat per capes, en el projecte api inclou els controllers, endpoints i configuracions, despres estarien les capes de library que inclou els serveis, dtos i enums on s'aplica la lògica i validacions, seguit de la capa de infrastructure on inclou els repositoris i entitats de BD que gestiona el accés a BD i finalment el projecte de tests.

Per la part de front fet en Angular se ha relitzat una interfcie per consultar les campanyes que existeixen, poder veure el seu estat, poder crear de noves(en estat esborrany), mostrar consums i pressupostos.
També he afegit validacions que milloren l'experiencia de l'usuari.

He utilitzat aquest patrons per mantenir una arquitectura ordenada i neta.

### 🚀 Millores futures
En un futur es podrien afegir noves funcionalitats i millores de accesibilitat de cara a l'usuari.
- el poder eliminar les campanyes manualment
- a la hora de crear una nova poder escollir l'estat(sense el finalitzada), com per exemple amb un checkbox on poguesis crearla activa ya.
(ara mateix per fer proves activo les campanyes desde BD amb un update ``UPDATE Campaigns SET Status = 'activa' where id = (posar guid))`` o amb el endpoint de actualitzar status campanya) 
- un buscador per filtrar campanyes coincretes quan hi hagues un gran volum de campanyes
- implementar autorització amb [Authorize] per restringir accés als endpoints segons els permisos de cada usuari


###  Promps que s'han utilizat 
--
Aplciacio .NET he utilitzat prompts basic de creació de estructures, copm per exemple:

'hazme la estructura basica para realizar aplicacion .net con controllers, capa de library para gestionar los servicios y capa de infrastructure para el tema de repositorios y DB'

'Detallame como conectar correctamente la base de dades sqlite con etity framework i configurar el dbcontext i implementar repositorios del proyecto'

utilitzat per realitzar les extensions per registrar els serveis amb el program.cs i injectar dependencies.
utilitzat per configurar els unitests.


Angular:

'estructurar una aplicacion Angular para gestionar campañas de marketing y mostrar su listado en una tabla'
'como conectar el frontend Angular con la api rest desarrollada en .NET para consultar, crear y actualizar campaña'
'mejorame el diseño de app html a un estilo mas moderno para que accesible para el usuario'
'controlar que el consumo de presupuesto solo esta disponible para campañas activas y dentro del periodo de startdate i enddate'

utilitzat per algun problema de refresh de la pagina al interactuar.
utilitzat per realitzar validacions i mostrar misastges de error.

--
