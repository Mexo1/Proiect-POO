Acest proiect reprezinta un sistem de gestiune a donatiilor pentru ONG-uri. Aplicatia permite gestionarea campaniilor de fudraising si a donatiilor realizate de utilizatori. Proiectul este structurat pe principii POO si DDD( Domain-Driven Design). 
Am fost responsabila de modelarea domeniului aplicatiei, incluzand:
      -definirea entitatilor;
      -definirea ValueObjects;
      -implementarea regulilor de business;
      -realizarea agregatelor;
      -tratarea exceptiilor de domeniu;
Aceasta parte este complet independenta de interfata, persistenta datelor sau infrastructura. Reprezinta nucleul aplicatiei si va fi utilizata de celelalte module pentru persistenta datelor,interfata utilizator,logare si configurare. 

Structura Domain-ului:

1)ValueObjects:

  ValueObject-urile sunt imutabile si contin validari.
    -Money:reprezinta o suma de bani(doar valori pozitive);
    -Email:valideaza formatul unui email;
    -CampaignId,DonationId,UserId-identificatori unici bazati pe Guid
    -Category:enum pentru tipurile de campanii(Education, Health, Environment, Social;
    
2)Entities:

Toate entitatile folosesc proprietati cu setteri privati sau doar cu get, asigurand incapsularea completa. Modificarea starii interne se face exclusiv prin metode autorizate. User(clasa abstracta) reprezinta baza sistemului de utilizatori. Este marcata ca abstract pentru a preveni instantierea directa, impunand validari esentiale(ex: numele nu poate fi gol) pentru orice tip de utilizator.
AdminONG: entitate specializata care mosteneste clasa User. Are responsabilitatea de a gestiona campaniile de fundraising si de a urmari donatiile primite, avand in plus proprietatea OrganizationName.
Donator: persoana fizica ce contribuie la cauze. Aceasta entitate permite urmarirea istoricului personal de donatii si vizualizarea impactului acestora.
Donation: eveniment unic de transfer financiar intre un donator si o campanie. include informatii despre summa, data realizari si identitatea donatorului, fiind imutabila.

3)Aggregates:

Entitatea Campaign actioneaza ca un Aggregate Root. Aceasta gestioneaza intern colectia de donatii si garanteaza integritatea datelor prin metode de control precum AddDonation(). Contine logica de business care inchide automat campania atunci cand obiectivul financiar (TargetAmount) a fost atins.

4)Exceptions:
    -DomainException:exceptie specifica domeniului, utilizata pentru validarea regulilor de business;


