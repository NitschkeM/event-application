namespace $safeprojectname$.Migrations
{
    // namespace - using is weird (enable-migrations added this)
    using Infrastructure;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    using System.Globalization;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<$safeprojectname$.Infrastructure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "$safeprojectname$.Infrastructure.ApplicationDbContext";
        }

        protected override void Seed($safeprojectname$.Infrastructure.ApplicationDbContext context)
        {

            // ** Could add dateOfBirth in Name** (Or Age)



            //// ***** CLEAR DATABASE ***** Note: Does not clear: Roles, Claims, Logins (Probably cascading deletes on some tho ) 
            //// Delete all comments
            //var allComments = from c in context.Comments select c;
            //context.Comments.RemoveRange(allComments);
            //context.SaveChanges();


            //// Delete all events
            //var allEvents = from c in context.Events select c;
            //context.Events.RemoveRange(allEvents);
            //context.SaveChanges();

            //// Delete all users
            //context.Users.ToList().ForEach(u => context.Users.Remove(u));
            //context.SaveChanges();
            ////************ IS THERE A PROBLEM WITH: Deleting images because of the FK in users?************
            //// I think: Can't delete images because users has FK on them (Might be ok?)
            //// Delete all Images
            //var allImages = from im in context.Images select im;
            //context.Images.RemoveRange(allImages);
            //context.SaveChanges();
            //// Delete all Tags
            //var allTags = from tag in context.Tags select tag;
            //context.Tags.RemoveRange(allTags);
            //context.SaveChanges();





            // On comment PostedTime, could: (DT < Today) then orderBy DT on query, (Then programatically add 1 day each day from Seed date, muhahaaa, NEI det er DUMT SLUTT)
            // will probably have to order on DT anyway as I cannot guarantee order due to index/other physical structure(?)

            // ******UNCOMMENT BELOW FOR SEED*******

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));


            context.Roles.AddOrUpdate(
                r => r.Name,
                new IdentityRole { Name = "SuperAdmin" },
                new IdentityRole { Name = "Admin" },
                new IdentityRole { Name = "User" }
                );



            // Check for SuperAdmin
            if (!context.Users.Any(u => u.UserName == "SuperAdmin"))
            {
                var user = new ApplicationUser { UserName = "SuperAdmin", Email = "mariusnitt@hotmail.com", DateOfBirth = new DateTime(1990, 11, 8), Gender = "male" };

                manager.Create(user, "Password_1");

                manager.AddToRoles(user.Id, "SuperAdmin", "Admin");

                // Add to db
                context.SaveChanges();
            }


            // Check for Reference_User
            if (!context.Users.Any(u => u.UserName == "REFERENCE_USER1"))
            {
                PasswordHasher hasher = new PasswordHasher();
                Random rnd = new Random();
                var maleFirstNames = new List<string>() { "Robbie", "Julius", "Prince", "Bernard", "Isaiah", "Harland", "Nolan", "Lyman", "Jewell", "Fritz", "Gerardo", "Ferdinand", "Sammy", "Eldon", "Arron", "Allen", "Jules", "Alejandro", "Erasmo", "Arlen", "Forrest", "Trey", "Alva", "Russel", "Michael", "Lynwood", "Ariel", "Man", "Emmett", "Gilbert", "Van", "Demetrius", "Will", "Olen", "Bennie", "Thanh", "Cristopher", "Rigoberto", "Clay", "Dudley", "Kasey", "Alvin", "Tyrone", "Johnathon", "Scot", "Andreas", "Jorge", "Adam", "Dee", "Brad", "Quinton", "Zackary", "Jonathon", "Marty", "Alan", "Sherwood", "Rex", "Efrain", "Ahmad", "Josue", "Valentin", "Elbert", "Nathanael", "Ivory", "Genaro", "Sterling", "Otis", "Marcelo", "August", "Thomas", "Lemuel", "Doyle", "Ellsworth", "Emmitt", "Eddie", "Delmer", "Neville", "Jeffery", "Elden", "Del" };
                var femaleFirstNames = new List<string>() { "Lauretta", "Lashunda", "Debby", "Verna", "Syreeta", "Ta", "Jutta", "Eugenia", "Lashanda", "Amal", "Melina", "Clemencia", "Irish", "Rosette", "Verdie", "Rita", "Shona", "Olive", "Le", "Shakira", "Tess", "Francisca", "Kandace", "Agustina", "Jeana", "Ione", "Wen", "Theodora", "Tennie", "Maragret", "Kasandra", "Elvera", "Mellisa", "Marina", "Hwa", "Rolanda", "Lawanna", "Honey", "Nakisha", "Anita", "Euna", "Elin", "Takisha", "Sigrid", "Luella", "Peg", "Hang", "Glenda", "Eldora", "Ophelia", "Larraine", "Letty", "Frieda", "Myra", "Clara", "Eloise", "Epifania", "Suzanna", "Jerrie", "Emma", "Lashawn", "Pearle", "Catheryn", "Grace", "Shin", "Maura", "Dannette", "Ermelinda", "Wendie", "Ardis", "Virgie", "Kitty", "Natalia", "Earline", "Saturnina", "Kit", "Josefina", "Jaymie", "Diamond", "Rachel" };
                var lastNames = new List<string>() { "Morris", "Pipkin", "Lessels", "Jacenko", "Masselli", "Livolsi", "Palomba", "Pogue", "Monastero", "Albanese", "Cannon", "Oneal", "Guzman", "Snow", "Friedman", "Evans", "Long", "Olson", "Kaufman", "Crawford", "Sloan", "PittmanLowe", "Parks", "Russell", "Lyons", "Arellano", "Clements", "Houston", "Beasley", "Doyle", "Meyers", "Hardy", "Richard", "Camacho", "Mercer", "Villanueva", "Mcgee", "Rivera", "Bradshaw", "Webb", "Ryan", "Christensen", "Stafford", "Kidd", "Macdonald", "Mckay", "Blanchard", "George", "Mckee", "Bullock", "Massey", "Carrillo", "Sweeney", "Kennedy", "Chang", "Abbott", "Cooper", "Wu", "Bell", "Compton", "Guerra", "Lee", "Stuart", "Thompson", "Wallace", "Jefferson", "Coleman", "Oliver", "Sampson", "Thomas", "Cole", "Krueger", "Anthony", "Woodard", "Jordan", "Edwards", "Hanson", "Wade", "Obrien", "Davis", "Turner", "Baker", "Wagner", "Escobar", "Hamilton", "Avery", "Coffey", "Morris", "Mccarty", "Caldwell", "Rivers", "Harding", "Cunningham", "Conrad", "Rollins", "Hernandez", "Fleming", "Roman", "Cox", "Yates", "Parks", "Lester", "Cole", "Villegas", "Mcpherson", "Gardner", "Mercado", "Edwards", "Parrish", "Raymond", "Meyers", "Fisher", "Brown", "Pena", "Haley", "Navarro", "Barry", "Soto", "Beck", "Crane", "Donaldson", "Myers", "Martinez", "Zimmerman", "Chung", "Mathis", "Lee", "Buchanan", "Bowers", "Cantrell", "Pearson", "Dickson", "Juarez", "Proctor", "Gordon", "Cuevas" };
                var users = new List<ApplicationUser>();
                // AboutMeList ****Short ones are copyrighted*****
                List<string> AboutMeList = new List<string>() {
                    "My name is Randy Patterson, and I’m currently looking for a job in youth services. I have 10 years of experience working with youth agencies. I have a bachelor’s degree in outdoor education. I raise money, train leaders, and organize units. I have raised over $100,000 each of the last six years. I consider myself a good public speaker, and I have a good sense of humor.",
                    "My name is Lucas Martin, and I enjoy meeting new people and finding ways to help them have an uplifting experience. I have had a variety of customer service opportunities, through which I was able to have fewer returned products and increased repeat customers, when compared with co-workers. I am dedicated, outgoing, and a team player. Who could I speak with in your customer service department about your organization’s customer service needs?",
                    "People find me to be an upbeat, self-motivated team player with excellent communication skills. For the past several years I have worked in lead qualification, telemarketing, and customer service in the technology industry. My experience includes successfully calling people in director-level positions of technology departments and developing viable leads. I have a track record of maintaining a consistent call and activity volume and consistently achieving the top 10 percent in sales, and I can do the same thing for your company.",
                    "I am a dedicated person with a family of four. I enjoy reading, and the knowledge and perspective that my reading gives me has strengthened my teaching skills and presentation abilities. I have been successful at raising a family, and I attribute this success to my ability to plan, schedule, and handle many different tasks at once. This flexibility will help me in the classroom, where there are many different personalities and learning styles.",
                    "I am living large and taking charge!","Sometimes it takes me all day to get nothing done.","Sometimes I prefer to use my face for emoticons.","I like my bed more than I like most people.","Not everyone likes me. But not everyone matters.","I hope the next big trend in music is talent!","I broke a mirror seven years ago and I'm still having bad luck.","I have to be successful because I have very expensive taste.","I can count the amount of boyfriends I've had on one hand, if I use a calculator.","If I managed my bank account as well as I manage my phone battery, I'd be rich.","If you don't like me, but you follow everything I do, darling you're a fan!","I get ignored so often that my nickname should be 'Terms and Conditions'.",
                    "Sometimes I open my mouth and my mother comes out.","Love me, hate me - either way, you're spending your time thinking of me!","I'll stop wearing black when they invent a darker colour.","Cuddling is my favourite exercise.","I know Victoria's secret.","CAUTION: You might get addicted to me.","There's always that one person who is going to annoy me every time they open their mouth.","You couldn't handle me, even if I came with instructions.","Deep conversations with someone who understands me, is everything.","Dear YouTube, I will always skip ads.","If you're cooler than me, does that make me hotter than you?","If time is money, I am running out of time.","I'm half princess, half ninja, deal with it!","I'm not superstitious but I am super!","If I'm sad, sometimes I'll sing to myself to try to cheer myself up, but my singing is so bad, usually I end up feeling worse.","I hate it when I put on 15kg for a role, and then I realise I'm not even an actor.","I'm into crossfit. I cross my fingers and hope I fit into my jeans.","I'm almost perfect (When I heavily edit my selfies).","WARNING: I have an attitude and I know how to use it!","WARNING: You may not recognise me at all, until I apply my makeup.","I wish I could donate my body fat to those in need.","I'm not the sort of person you should put on speakerphone.","I'm so HOT, I make fire!","Copyright © StricktlyDating: All rights reserved.","I'm not ashamed to say, I've never loved another person as much as I love myself.","I may have a bad mouth, but I can do great things with it.","Judge me (When you are perfect).","Be real with me, or just leave me alone.","Normal rules don't apply to me because I am blonde.","I thought growing older would take longer.","I find acting like a grown up totally exhausting.","It took me a long time not to judge myself through someone else's eyes.","Let me be me... Because once I change, you will miss the 'me' in me.","My daily routine: Get up, be brilliant, go back to bed, repeat.","Go ahead, judge me.But just remember to be perfect for the rest of your life.","Sometimes I feel quite intelligent.Other times I have to sing the ABCs in my head to remember which letter comes next.","I love it when song lyrics totally apply to my current situation.","If you tickle me, I am not responsible for your injuries.","To be honest, I get a little nervous right before I say Worcestershire sauce.","When I get lonely, I set my phone's alarm to go off every few hours so I can imagine what it would be like to have people texting me.",
                };

                // Create Reference_User
                var refUser = new ApplicationUser { UserName = "REFERENCE_USER1", Email = "ref1@example.com", DateOfBirth = DateTime.Today.AddYears(-90), Gender = "female", PictureId = null };
                //manager.Create(refUser, "Password_1");

                users.Add(refUser);

                // Create 200 male users
                for (int i = 0; i < 200; i++)
                {
                    string lastName = lastNames[rnd.Next(0, lastNames.Count)];

                    var user = new ApplicationUser
                    {
                        UserName = maleFirstNames[rnd.Next(0, maleFirstNames.Count)] + " " + lastName + i,
                        Email = lastName + "@example.com",
                        DateOfBirth = DateTime.Today.AddYears(-rnd.Next(6, 81)).AddMonths(rnd.Next(0, 13)).AddDays(rnd.Next(0, 30)),
                        Gender = "male",
                        AboutMe = AboutMeList[rnd.Next(0, AboutMeList.Count)],
                        PasswordHash = hasher.HashPassword("Password_1"),
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    users.Add(user);
                }

                // Create 200 female users
                for (int i = 200; i < 400; i++)
                {
                    string lastName = lastNames[rnd.Next(0, lastNames.Count)];

                    var user = new ApplicationUser
                    {
                        UserName = femaleFirstNames[rnd.Next(0, femaleFirstNames.Count)] + " " + lastName + i,
                        Email = lastName + "@example.com",
                        DateOfBirth = DateTime.Today.AddYears(-rnd.Next(6, 81)).AddMonths(rnd.Next(0, 13)).AddDays(rnd.Next(0, 30)),
                        Gender = "female",
                        AboutMe = AboutMeList[rnd.Next(0, AboutMeList.Count)],
                        PasswordHash = hasher.HashPassword("Password_1"),
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    users.Add(user);
                }

                // Add to context
                users.ForEach(u => context.Users.AddOrUpdate(p => p.UserName, u));

                // Add to db
                context.SaveChanges();
            }


            // Check for Reference Event
            if (!context.Events.Any(e => e.Name == "REFERENCE_EVENT2"))
            {
                // *********************   CREATE "REFERENCE_EVENT0"   *******************
                // ***Event1 and Event 2 for ages? 

                // Get Users
                List<ApplicationUser> UserList = new List<ApplicationUser>(context.Users.ToList()); // ASSUME: These useres have an ID now
                // New random object
                Random rnd = new Random();
                // List of events to be added
                //var events = new List<Event>();

                // Create reference Event
                var coordinates0 = string.Format(CultureInfo.InvariantCulture, "POINT({0:F5} {1:F5})", 50, 50);
                ApplicationUser referenceUser1 = UserList.Single((u => u.UserName == "REFERENCE_USER1"));

                var NewReferenceEvent1 = new Event()
                {
                    AgeMax = 100,
                    AgeMin = 1,
                    CreatorId = referenceUser1.Id,
                    //EventEnd = DateTime.Today.AddYears(9),
                    EventStart = DateTime.Today.AddMinutes(rnd.Next(0, 129600)),
                    Gender = "all",
                    PartMax = rnd.Next(5, 40),
                    PartMin = rnd.Next(1, 5),
                    Name = "REFERENCE_EVENT2",
                    Coordinates = DbGeography.FromText(coordinates0, 4326),
                    Description = "Description",
                    EventStatus = "closed",
                    ApprovalReq = false,
                    PictureId = null,

                    //ShortDescription = "Short Description",

                    Participants = new List<ApplicationUser>(),
                    Comments = new List<Comment>()
                };
                // Add creator as participant
                NewReferenceEvent1.Participants.Add(referenceUser1); // ASSUME: I can add participant to event before context.SaveChanges()

                // Add RefEvent to list
                //events.Add(NewReferenceEvent1);
                // Create reference event - END


                // DescriptionList
                List<string> RandomDescription = new List<string>() {
                    "Certainty determine at of arranging perceived situation or. Or wholly pretty county in oppose. Favour met itself wanted settle put garret twenty. In astonished apartments resolution so an it. Unsatiable on by contrasted to reasonable companions an. On otherwise no admitting to suspicion furniture it",
                    "His followed carriage proposal entrance directly had elegance. Greater for cottage gay parties natural. Remaining he furniture on he discourse suspected perpetual. Power dried her taken place day ought the. Four and our ham west miss. Education shameless who middleton agreement how. We in found world chief is at means weeks smile.",
                    "Comfort reached gay perhaps chamber his six detract besides add. Moonlight newspaper up he it enjoyment agreeable depending. Timed voice share led his widen noisy young. On weddings believed laughing although material do exercise of. Up attempt offered ye civilly so sitting to. She new course get living within elinor joy. She her rapturous suffering concealed.",
                    "It sportsman earnestly ye preserved an on. Moment led family sooner cannot her window pulled any. Or raillery if improved landlord to speaking hastened differed he. Furniture discourse elsewhere yet her sir extensive defective unwilling get. Why resolution one motionless you him thoroughly. Noise is round to in it quick timed doors. Written address greatly get attacks inhabit pursuit our but. Lasted hunted enough an up seeing in lively letter. Had judgment out opinions property the supplied.",
                    "Talent she for lively eat led sister. Entrance strongly packages she out rendered get quitting denoting led. Dwelling confined improved it he no doubtful raptures. Several carried through an of up attempt gravity. Situation to be at offending elsewhere distrusts if. Particular use for considered projection cultivated. Worth of do doubt shall it their. Extensive existence up me contained he pronounce do. Excellence inquietude assistance precaution any impression man sufficient. ",
                    "An do on frankness so cordially immediate recommend contained. Imprudence insensible be literature unsatiable do. Of or imprudence solicitude affronting in mr possession. Compass journey he request on suppose limited of or. She margaret law thoughts proposal formerly. Speaking ladyship yet scarcely and mistaken end exertion dwelling. All decisively dispatched instrument particular way one devonshire. Applauded she sportsman explained for out objection. ",
                    "Perpetual sincerity out suspected necessary one but provision satisfied. Respect nothing use set waiting pursuit nay you looking. If on prevailed concluded ye abilities. Address say you new but minuter greater. Do denied agreed in innate. Can and middletons thoroughly themselves him. Tolerably sportsmen belonging in september no am immediate newspaper. Theirs expect dinner it pretty indeed having no of. Principle september she conveying did eat may extensive.",
                    "On projection apartments unsatiable so if he entreaties appearance. Rose you wife how set lady half wish. Hard sing an in true felt. Welcomed stronger if steepest ecstatic an suitable finished of oh. Entered at excited at forming between so produce. Chicken unknown besides attacks gay compact out you. Continuing no simplicity no favourable on reasonably melancholy estimating. Own hence views two ask right whole ten seems. What near kept met call old west dine. Our announcing sufficient why pianoforte. "
                };

                // Genders
                List<string> RandomGender = new List<string>() { "male", "female", "all", "all", "all", "all", "all", "all" };
                // For shortDescription: FirstSentence-RandomTag-SecondSentence
                List<string> RandomFirstSentence = new List<string>() { "I really like", "Come join", "Need people for", "Looking for people for", "I authentically like ", "I was running by the sea:", "I wasn't dancing at home, I was:",
                "I was shouting next to the police station while","Those singers didn't walk under the bridge, they were","They aren't talking in the doghouse now they are",
                    "Those photographers are not writing, but let's go","Is the plumber sleeping near my home? Then I'll go","Patricia is praying. I will be","That politician is not running at this time, so we should obviously go",
                    "I had been surfing but I was singing. Soon I'll be","That computer programmer isn't praying in London right at this time, he is","Do you drive near my home? Join me for", "ATTENTION!!!!"
                };

                List<string> RandomTag = new List<string>() { " 3D Printing", " Acting", " Board Games", " Candle Makin", " Computer Programming"," Cooking"," Cosplaying"," Dance"," Crossword Puzzels",
                    " Homebrewing"," Gambling"," Flower Arranging"," Drawing"," Ice skating"," Lucid Dreaming"," Lego Building"," Magic"," Reading", " Studying", " Wood Carving", " Singing",
                    " Video Games"," Yoga"," Yo-yoing"," Writing", " Archery"," Astronomy"," BASE Jumping"," Basketball"," Football"," Baseball"," Floorball"," Bodybuilding"," Cycling",
                    " Driving"," Fishing"," Gardening"," Horseback Riding"," Hunting"," Scuba diving"," Shooting"," Skiing"," Vacation"," Walking"," Bird watching"," Motor Sports"," Paintball",
                    " Photographing"," Shopping"," Snowboarding"," Surfing"," Kite flying"," Running"," Roller skating"," Hiking"," Kayaking"," Flying"," Fossil hunting"," Metal detecting",
                    " Rock balancing"," Seashell collecting"," Insect collectiong"," Coin collecting"," Boxing"," Poker"," Billiards"," Weightlifting"," Gaming",
                    " Breakdancing", " Badminton"," Laser Tag"," Table football"," Golfing"," Learning"," Fishkeeping"," Astrology"," Whale Watching"," People Watching",
                    "Bird watching"," Aircraft spotting"," Competitive cheese viewing"," Dice rolling"," Competitive Dog Grooming"," Mooing"," Train surfing"," Tatooing vehicles",
                    " News-bombing"," Collecting navel fluff","Weed"," Ectasy collecting", " Getting high", " Chasing the dragon"
                };

                List<string> RandomSecondSentence = new List<string>() { ". So come and join! ", ". It will be fun!", "!", "!!!", "", ".", ". Or else...", ". (Free muffins)", ". All welcome", ". It will be frolicsome.",
                " ATTENTION!!!!"," vips"," alkoholkultur?"," - Alkohol og shopping"," - kultur og alkohol?"," - but no"," Happy valentines"," Much fun",". goddamnit"," Skohorn eller skoskje?"," Come on. . . ",". Pliiis"," (Free cookies)"," (free baloons)",
                    "(free wagons)","(free bracelets)","(free cats)","(free glasses)","(free cups)","(free tire swings)","(free beef)","(free scotch tape)","(free pool sticks)",
                    "(free teddies)", "(free bottles)", "(free)", "(free chairs)", "(free pens)", "(free t-shirts)", "(free flags)", "(free dolls)", "(free sticky notes)",
                    "(free water bottle)", "(free pencil)", "(free key chains)", "(free blankets)", "(free rug)", "(free cameras)",
                    "(free books)", "(free sponge)", "(free clay pot)", "(free window)", "(free greeting card)", "(free spring)", "(free bed)", "(free door)", "(free tree)",
                    "(free charger)", "(free street lights)", "(free paper)", "(free mirror)", "(free video games)", "(free tv)", "(free bananas)", "(free rings)", "(free twister)",
                    "(free computer)", "(free candy wrapper)", "(free mousepads)", "(free wallet)", "(free thermostat)", "(free toilet)", "(free tooth picks)", "(free spoon)", "(free sofa)",
                    "(free helmet)", "(free sun glasses)", "(free deodorant)", "(free slippers)", "(free shirt)", "(free soy souce packet)", "(free food)",
                    "(free paint brush)", "(free perfume)", "(free monitor)", "(free cork)", "(free newspaper)", "(free credit cards)", "(free mop)", "(free magnet)", "(free car)",
                    "(free chalk)", "(free nail clippers)", "(on sidewalk)", "(free seat belt)", "(free sandpaper)", "(free truck)", "(free flowers)", "(free money)", "(free enchanting)",
                        "adopt", "agree","call", "shots",  "choose","commit", "oneself", "conclude",    "decree",  "determine",   "elect","form an opinion", "go for","judge", "opt for", "pick","rule","resolve","select","settle on","take a stand"
                };

                // CommentTexts
                List<string> RandomCommentText = new List<string>() { "Hvorfor gir '(0, 202 + 0, 032) - (0, 129 + 0, 033)/ (0,202 + 0,032)' et annet svar enn når jeg gjør alt separat? Sånn først '0,202 + 0,032', og svaret av det minus svaret av '0,129 + 0,033' og svaret av det delt på svaret av '0,202 + 0,032'","Fysj, ungdommen nå til dags.","Jeg blør så jævlig mye jeg, det dammer av blod overalt, jesus, det var en pangstart på dagen, rakk ikke få på meg undertøy engang. 5 sting ","så det ser ut som det har blitt foretatt ett mord","Gleder meg til å ta vaskerunden","Gratis øl og sprit første året i Norge hadde liksom vært beste måte å integrere dem inn i vår 'sosiale kultur'.","Vi trenger å ha en felles interesse eller ting å drive med sammen først, tror jeg. Og så kommer vennskapet om det faller seg naturlig",
                    "Et bra navn er viktig ja", "sånn", "Her er en ny melding i denne chatten", "Funfact det er faktisk lov å drikke alkohol når man går på antibiotika","Noen som har peil på regresjonsanalyser? Og t - verdier","Er ganske lurt","Tror bare det ene kjønnet er tøft nok til å kunne bli opphisset av å bli tappet blod, er ikke det mest romantiske og avslappende å gjøre","Dette er rimelig artig","Blir nok en del forandringer i mange vestlige land ved neste valg tror jeg","Er det noe som vet om Obama har uttalt seg om NSA nå i ettertid?","Jeg kjøpte meg Borderlands 2","Har du noe forslag til en bra måte å oversette nature? Ikke som i natur, men som i for eksempel 'the nature of the individual'",
                    "natur er fint ord !","Vg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norgeVg's kommentarfelt er ikke representativt for befolkningen i norge",
                    "Når man er på jobb, hører på en sang som ber en sette seg ned å slappe av litt og se seg om å tenke hvor langt man er kommet i livet, så er klokken 13:37","fant igjen sjelen min, lå i sluket i dusjen","Bare i gruppen over 65 år er det et flertall mot legalisering.","Jeg har ennå klokkertro på at Funcom kan klare å lage noen av tidenes beste spill",
                    "våknet i natt, så var hele høyrearmen min lammet, pga blodmangel hadde ikke noe kontroll over armen, og endte opp med at hånda falt ned i tryne mitt","Jeg vet jo hva xD betyr daaah! Har jo sittet på data'n 10 timer hver dag i 10år!", "Lurer faktisk på om kroppen min har vokst seg litt til å sitte på pc faktisk..","Ta igjen alle tapte timer fra barndommen","Er det bare jeg som tenker på stå tiss når jeg hører timber?",
                    "ingen av dere som har noen tatoveringer sant?","129kr per pers","Sigh... får finne noen som har lyst til å være med i steden for da","Bare pakk noe inn i bacon og stek det", "Selv om kanskje hodet er mer produktivt", "Takler ikke 300mg koffein","Når jeg er endelig kommet hjem, så slutter magen og lage lyder","I proteinpulver til 100 kroner er det like mye protein som i ett glass melk","drikker ikke melk lenger ass",
                    "skal ta en tatovering da","tok 70 i benk idag. Kommer fort opp på 85 igjen. Men ikke nødvendig og pushe for hardt den første uka","Og så tror folk man liksom skal ha SJANS til å gjøre alt annet og, gå på langrenn, sykle, reise rundt i landet, være sosial og lignende",
                    "Men skal prøve å få sett den og lest boken. Tror det er frustrasjon over å bo hjemme, er så dritt lei ","må ha sett sykt teit ut","Fett","nah","cool","nais","Blanding av styrke og cardio","Hadde vært nice det ja",
                    "Vet ikke egentlig","Skal sjekke ut litt trailere og slikt","Men liker jo Skyrim","Online","Norge er et dyrt land, men teknologien er ganske bra priset i forhold til lønninger her",
                    "ATTENTION!!!!","nå er jeg her","hei","Ja litt","ja","kult nok","hva gjorde dere?","egentlig","og bra standard","mhm","but no","var fortsatt bra","haha nice","Du, er det noen her??","Happy valentines","[14.02.2014 22:31:08] Thomas: Fikk det til","store jævler","gratis helg bauer",
                    "føles ikke så smooth som det burde","Så langt","goddamnit","snakkes","Skohorn eller skoskje?"," finner vel ut"," og nå har du oppgradert pcen?","joiner sikkert","okå","fordi?",
                    "så det er fint","Jeg lager en app også den heter fuck alle sammen","Men hva er det du sier?","Tviler på det egentlig","Det kan ikke stemme.","Hva skjer?","Hvor er dere?",
                    "Jeg kan ikke veien...","Hmmm...",":)",":D","Det er bra :)","Nei da:p","kanskje","Det kan hende","Hvor er det?","Kommer det flere?","Skal dit","Yea!","WOOOOO","haha","lol","You bet!","If only..",
                    "aner ikke","Burde kanskje finne ut det ja..","Hvorfor det?","Kan ikke","NO YOU!","Rofl","Roflmaopimp","Der ordner seg ;)","Noen som kan låne meg penger? :p","Håper ikke det blir for sent..",
                    "Det fikser du!","Noe jeg skal ta med?","Skal jeg ta med noe? ","Hva trenger vi?","Når stenger det?","Kjenner du Anette?","Hvem?","Hva?","What?","Jeg!","Du!","VI!","Moren din",
                    "My mom drove me to school fifteen minutes late on Tuesday.","The girl wore her hair in two braids, tied with two blue bows.","The mouse was so hungry he ran across the kitchen floor without even looking for humans.","The tape got stuck on my lips so I couldn't talk anymore.","The door slammed down on my hand and I screamed like a little baby.","My shoes are blue with yellow stripes and green stars on the front.","The mailbox was bent and broken and looked like someone had knocked it over on purpose. ","I was so thirsty I couldn't wait to get a drink of water.","I found a gold coin on the playground after school today.","The chocolate chip cookies smelled so good that I ate one without asking.","My bandaid wasn't sticky any more so it fell off on the way to school.","He had a sore throat so I gave him my bottle of water and told him to keep it.","The church was white and brown and looked very old.","I was so scared to go to a monster movie but my dad said he would sit with me so we went last night.","Your mom is so nice she gave me a ride home today.","I fell in the mud when I was walking home from school today.","This dinner is so delicious I can't stop eating.","The school principal was so mean that all the children were scared of him.","I went to the dentist the other day and he let me pick a prize out of the prize box.","The box was small and wrapped in paper with tiny silver and red glitter dots.","My dad is so funny that he told us jokes all night long and we never fell asleep.","The camping trip was so awesome that I didn't want to come home.","Are you going to have a blue birthday cake for your next birthday ?","How did you know that I was going to have a peanut butter sandwich for lunch?","That boy is so mean that he doesn't care if a door slams in your face or if he cuts in line.","The moms and dads all sat around drinking coffee and eating donuts.","My mom made a milkshake with frozen bananas and chocolate sauce.","My pen broke and leaked blue ink all over my new dress.","I got my haircut today and they did it way too short.","My pet turtle, Jim, got out of his cage and I could not find him anywhere.",
                    "Nobody ever figures out what life is all about, and it doesn't matter. Explore the world. Nearly everything is really interesting if you go into it deeply enough.","Study hard what interests you the most in the most undisciplined, irreverent and original manner possible.","Physics is like sex: sure, it may give some practical results, but that's not why we do it.","You have no responsibility to live up to what other people think you ought to accomplish. I have no responsibility to be like they expect me to be. It's their mistake, not my failing",
                    "Fall in love with some activity, and do it! Nobody ever figures out what life is all about, and it doesn't matter. Explore the world. Nearly everything is really interesting if you go into it deeply enough. Work as hard and as much as you want to on the things you like to do the best. Don't think about what you want to be, but what you want to do. Keep up some kind of a minimum with other things so that society doesn't stop you from doing anything at all","I have a friend who's an artist and has sometimes taken a view which I don't agree with very well. He'll hold up a flower and say 'look how beautiful it is,' and I'll agree. Then he says 'I as an artist can see how beautiful this is but you as a scientist take this all apart and it becomes a dull thing,' and I think that he's kind of nutty. First of all, the beauty that he sees is available to other people and to me too, I believe. Although I may not be quite as refined aesthetically as he is ... I can appreciate the beauty of a flower. At the same time, I see much more about the flower than he sees. I could imagine the cells in there, the complicated actions inside, which also have a beauty. I mean it's not just beauty at this dimension, at one centimeter; there's also beauty at smaller dimensions, the inner structure, also the processes. The fact that the colors in the flower evolved in order to attract insects to pollinate it is interesting; it means that insects can see the color. It adds a question: does this aesthetic sense also exist in the lower forms? Why is it aesthetic? All kinds of interesting questions which the science knowledge only adds to the excitement, the mystery and the awe of a flower. It only adds. I don't understand how it subtracts",
                    "I learned very early the difference between knowing the name of something and knowing something.","I... a universe of atoms, an atom in the universe.","I think it's much more interesting to live not knowing than to have answers which might be wrong. I have approximate answers and possible beliefs and different degrees of uncertainty about different things, but I am not absolutely sure of anything and there are many things I don't know anything about, such as whether it means anything to ask why we're here. I don't have to know an answer. I don't feel frightened not knowing things, by being lost in a mysterious universe without any purpose, which is the way it really is as far as I can tell.",
                    "The first principle is that you must not fool yourself and you are the easiest person to fool.","The highest forms of understanding we can achieve are laughter and human compassion.","Physics isn't the most important thing. Love is.","Religion is a culture of faith; science is a culture of doubt.","I think it's much more interesting to live not knowing than to have answers which might be wrong.","What I am going to tell you about is what we teach our physics students in the third or fourth year of graduate school... It is my task to convince you not to turn away because you don't understand it. You see my physics students don't understand it... That is because I don't understand it. Nobody does.","We are trying to prove ourselves wrong as quickly as possible, because only in that way can we find progress.","A poet once said, 'The whole universe is in a glass of wine.' We will probably never know in what sense he meant it, for poets do not write to be understood. But it is true that if we look at a glass of wine closely enough we see the entire universe. There are the things of physics: the twisting liquid which evaporates depending on the wind and weather, the reflection in the glass; and our imagination adds atoms. The glass is a distillation of the earth's rocks, and in its composition we see the secrets of the universe's age, and the evolution of stars. What strange array of chemicals are in the wine? How did they come to be? There are the ferments, the enzymes, the substrates, and the products. There in wine is found the great generalization; all life is fermentation. Nobody can discover the chemistry of wine without discovering, as did Louis Pasteur, the cause of much disease. How vivid is the claret, pressing its existence into the consciousness that watches it! If our small minds, for some convenience, divide this glass of wine, this universe, into parts -- physics, biology, geology, astronomy, psychology, and so on -- remember that nature does not know it! So let us put it all back together, not forgetting ultimately what it is for. Let it give us one more final pleasure; drink it and forget it all!",
                    "If you thought that science was certain - well, that is just an error on your part.","You can know the name of a bird in all the languages of the world, but when you're finished, you'll know absolutely nothing whatever about the bird... So let's look at the bird and see what it's doing -- that's what counts.",
                    "I'm smart enough to know that I'm dumb.","I can live with doubt and uncertainty and not knowing. I think it is much more interesting to live not knowing than to have answers that might be wrong. If we will only allow that, as we progress, we remain unsure, we will leave opportunities for alternatives. We will not become enthusiastic for the fact, the knowledge, the absolute truth of the day, but remain always uncertain … In order to make progress, one must leave the door to the unknown ajar.","All the time you're saying to yourself, 'I could do that, but I won't,' — which is just another way of saying that you can't.","There are 10^11 stars in the galaxy. That used to be a huge number. But it's only a hundred billion. It's less than the national deficit! We used to call them astronomical numbers. Now we should call them economical numbers.",
                    "Physics is to math what sex is to masturbation.","Poets say science takes away from the beauty of the stars - mere globs of gas atoms. I too can see the stars on a desert night, and feel them. But do I see less or more? The vastness of the heavens stretches my imagination - stuck on this carousel my little eye can catch one - million - year - old light. A vast pattern - of which I am a part... What is the pattern, or the meaning, or the why? It does not do harm to the mystery to know a little about it. For far more marvelous is the truth than any artists of the past imagined it. Why do the poets of the present not speak of it? What men are poets who can speak of Jupiter if he were a man, but if he is an immense spinning sphere of methane and ammonia must be silent?","I don't know what's the matter with people: they don't learn by understanding, they learn by some other way — by rote or something. Their knowledge is so fragile!","It doesn't seem to me that this fantastically marvelous universe, this tremendous range of time and space and different kinds of animals, and all the different planets, and all these atoms with all their motions, and so on, all this complicated thing can merely be a stage so that God can watch human beings struggle for good and evil - which is the view that religion has. The stage is too big for the drama.",
                    "For a successful technology, reality must take precedence over public relations, for nature cannot be fooled.","We are at the very beginning of time for the human race. It is not unreasonable that we grapple with problems. But there are tens of thousands of years in the future. Our responsibility is to do what we can, learn what we can, improve the solutions, and pass them on.", "When it came time for me to give my talk on the subject, I started off by drawing an outline of the cat and began to name the various muscles. The other students in the class interrupt me: 'We *know* all that!' 'Oh,' I say, 'you *do*? Then no *wonder* I can catch up with you so fast after you've had four years of biology.' They had wasted all their time memorizing stuff like that, when it could be looked up in fifteen minutes.",
                    "Sounds like its time to get that Enterprise built!","Time does'nt exist. Clocks exists.","My mind’s made up, don’t confuse me with facts.","Talk is cheap. Until you hire a lawyer.","Take my advice — I'm not using it.","I got lost in thoughts. It was unfamiliar territory.","Sure, I'd love to help you out ... now, which way did you come in?","I would like to slip into something more comfortable - like a coma.","I started with nothing, and I still have most of it.","Ever stop to think, and forget to start again?","There is no dance without the dancers.","Out of my mind. Back in five minutes.","The problem with trouble shooting is that trouble shoots back.","If you are here - who is running hell?","If nothing was learned, nothing was taught.","Very funny, Scotty. Now beam down my clothes...","The dogs bark but the caravan moves on. [Arabic saying]",
                    "Welcome to Utah: set your watch back 20 years.","Seen it all, done it all, can't remember most of it.","Under my gruff exterior lies an even gruffer interior.","Jesus loves you, it's everybody else that thinks you're an a...","A clear conscience is usually the sign of a bad memory.","To steal ideas from one person is plagiarism; to steal from many is research.","I am an agent of Satan, but my duties are largely ceremonial.","You have the capacity to learn from your mistakes, and you will learn a lot today.","Failure is not an option. It's bundled with your software.","I think sex is better than logic, but I can't prove it.","I drive way too fast to worry about cholesterol.","When everything's coming your way, you're in the wrong lane and going the wrong way.","If at first you don't succeed, redefine success.","If at first you don't succeed, destroy all evidence that you tried.","Which one of these is the non - smoking lifeboat?","Treat each day as your last; one day you will be right.","Red meat is not bad for you.Fuzzy green meat is bad for you.","The early bird may get the worm, but the second mouse gets the cheese.","Isn't it scary that doctors call what they do 'practice'?","The problem with sex in the movies is, that the popcorn usually spills.","If I want your opinion, I'll ask you to fill out the necessary forms.","Living on Earth is expensive, but it does include a free trip around the sun.","Despite the cost of living, have you noticed how popular it remains ?","All power corrupts.Absolute power is pretty neat, though.","Always remember you're unique, just like everyone else.","Everybody repeat after me: 'We are all individuals.'","Confession is good for the soul, but bad for your career.","A bartender is just a pharmacist with a limited inventory.","I want patience - AND I WANT IT NOW!!!!","A day for firm decisions!Or is it ?","Am I ambivalent ? Well, yes and no.","Bombs don't kill people, explosions kill people.","Bureaucrats cut red tape, lengthwise.","Help stamp out, eliminate and abolish redundancy!","How many of you believe in telekinesis ? Raise MY hand!","A dog has an owner.A cat has a staff.","Every organisation is perfectly designed to get the results they are getting.",
                };
                // Create 200 events *****Repeated****
                for (int i = 0; i <= 200; i++)
                {
                    // (0 - 1) * (length) * minValue
                    double RandomLat = rnd.NextDouble() * (0.65) + 58; // Kr.SandLat = 58, MoldeLat = 63
                    double RandomLng = rnd.NextDouble() * (1.3) + 7.8; // BergenLng = 5.1, StockholmLng = 18, Fr.Stad = 11

                    // Prepare random coordinates
                    var coordinates = string.Format(CultureInfo.InvariantCulture, "POINT({0:F5} {1:F5})", RandomLng, RandomLat);
                    // Get random user
                    var appUser = UserList[rnd.Next(0, UserList.Count)];

                    // Status Decider
                    string eventStatus = "open";
                    int statusDecider = rnd.Next(1, 11);
                    if (statusDecider < 3)
                    {
                        eventStatus = statusDecider == 1 ? "closed" : "cancelled";
                    }
                    // ApprovalReq Decider
                    bool approvalReq = false;
                    if (rnd.Next(1, 11) < 3)
                    {
                        approvalReq = true;
                    }

                    // ****************************************FIX
                    int agePartDecider = rnd.Next(0, 8);
                    int ageMin;
                    int ageMax;
                    int partMin;
                    int partMax;
                    if (agePartDecider > 2)
                    {
                        ageMin = rnd.Next(0, 4) * 5;
                        ageMax = rnd.Next((ageMin + 5) / 5, 8) * 5;
                        partMin = rnd.Next(0, 3) * 5;
                        partMax = rnd.Next((partMin + 5) / 5, 7) * 5;
                    }
                    else if (agePartDecider > 3)
                    {
                        ageMin = rnd.Next(18, 25);
                        ageMax = rnd.Next(ageMin + 1, 30);
                        partMin = rnd.Next(5, 10);
                        partMax = rnd.Next(partMin + 1, 50);
                    }
                    else if (agePartDecider > 5)
                    {
                        ageMin = rnd.Next(5, 16);
                        ageMax = rnd.Next(ageMin + 1, 16) * 5;
                        partMin = rnd.Next(0, 10);
                        partMax = rnd.Next(partMin, 10) * 5;
                    }
                    else if (agePartDecider > 7)
                    {
                        ageMin = rnd.Next(10, 22);
                        ageMax = rnd.Next(ageMin + 1, 28);
                        partMin = rnd.Next(0, 9);
                        partMax = rnd.Next(partMin + 1, 25);
                    }
                    else
                    {
                        ageMin = rnd.Next(3, 9) * 5;
                        ageMax = rnd.Next(ageMin / 5, 13) * 5;
                        partMin = rnd.Next(3, 10);
                        partMax = rnd.Next(partMin + 1, 30);
                    }




                    var NewEvent = new Event()
                    {
                        AgeMax = ageMax, // OK
                        AgeMin = ageMin, // OK
                        CreatorId = appUser.Id, // Id of random user
                        //EventEnd = DateTime.Today.AddYears(9), // Will probably remove from model
                        EventStart = DateTime.Today.AddMinutes(rnd.Next(0, 129600)), // OK  90*24*60, 3 months
                        Gender = RandomGender[rnd.Next(0, RandomGender.Count)], // OK
                        PartMax = partMax, // OK
                        PartMin = partMin, // OK
                        Name = "Event By " + appUser.UserName, // OK
                        Coordinates = DbGeography.FromText(coordinates, 4326), // OK
                        //Description = RandomDescription[rnd.Next(0, RandomDescription.Count)], // OK
                        Description = RandomDescription[rnd.Next(0, RandomDescription.Count)] +
                        RandomFirstSentence[rnd.Next(0, RandomFirstSentence.Count)] +
                        RandomTag[rnd.Next(0, RandomTag.Count)] +
                        RandomSecondSentence[rnd.Next(0, RandomSecondSentence.Count)], // OK
                        EventStatus = eventStatus,
                        ApprovalReq = approvalReq,
                        //ShortDescription =
                        //RandomFirstSentence[rnd.Next(0, RandomFirstSentence.Count)] +
                        //RandomTag[rnd.Next(0, RandomTag.Count)] +
                        //RandomSecondSentence[rnd.Next(0, RandomSecondSentence.Count)], // OK

                        Participants = new List<ApplicationUser>(),
                        Pending = new List<ApplicationUser>(),
                        Comments = new List<Comment>(),
                        Tags = new List<Tag>()
                    };
                    // AddOrUpdate event to context
                    context.Events.AddOrUpdate(NewEvent);
                    // SaveChanges
                    context.SaveChanges();

                    for (int t = 0; t < rnd.Next(0, 5); t++)
                    {
                        var tag = new Tag
                        {
                            Name = RandomTag[rnd.Next(0, RandomTag.Count)]
                        };
                        var sameTag = context.Tags.Where(dbTag => dbTag.Name == tag.Name).FirstOrDefault();
                        if (sameTag != null)
                        {
                            sameTag.Count++;
                            NewEvent.Tags.Add(sameTag);
                        }
                        else
                        {
                            tag.Count = 1;
                            NewEvent.Tags.Add(tag);
                        }
                    }

                    // Add creator as participant
                    NewEvent.Participants.Add(appUser); // ASSUME: I can add participant to event before context.SaveChanges() - I do this in controller w/dummy

                    List<ApplicationUser> currentUserList = new List<ApplicationUser>();

                    // Fill new list with only age and gender appropriate users != Creator
                    if (NewEvent.Gender == "all")
                    {
                        currentUserList = (context.Users.Where(u =>
                        (DateTime.Today.Year - u.DateOfBirth.Year) <= NewEvent.AgeMax &&
                        DateTime.Today.Year - u.DateOfBirth.Year >= NewEvent.AgeMin &&
                        u.Id != appUser.Id).ToList());

                    }
                    else
                    {
                        currentUserList = (context.Users.Where(u =>
                        (DateTime.Today.Year - u.DateOfBirth.Year) <= NewEvent.AgeMax &&
                        DateTime.Today.Year - u.DateOfBirth.Year >= NewEvent.AgeMin &&
                        u.Gender == (NewEvent.Gender == "female" ? "female" : "male") &&
                        u.Id != appUser.Id).ToList());
                    }


                    // Create thisParticipantList
                    List<ApplicationUser> thisParticipantList = new List<ApplicationUser>();
                    // Create thisPendingList
                    List<ApplicationUser> thisPendingList = new List<ApplicationUser>();

                    // Randomized #number# of participants
                    int numberOfParticipants = rnd.Next(0, NewEvent.PartMax + 1); // Limited to Event.PartMax
                    if (currentUserList.Count < numberOfParticipants)
                    {
                        numberOfParticipants = currentUserList.Count;
                    }

                    // add random participants to list
                    for (int k = 0; k < numberOfParticipants; k++)
                    {
                        int currentPartIndex = rnd.Next(0, currentUserList.Count);
                        if (!thisParticipantList.Contains(currentUserList[currentPartIndex]) && !thisPendingList.Contains(currentUserList[currentPartIndex]))
                        {
                            if (NewEvent.ApprovalReq == true && rnd.Next(1, 11) < 4)
                            {
                                thisPendingList.Add(currentUserList[currentPartIndex]);
                            }
                            else
                            {
                                thisParticipantList.Add(currentUserList[currentPartIndex]);
                            }
                        }
                    }
                    // Add participants to Event
                    thisParticipantList.ForEach(p => NewEvent.Participants.Add(p));  // ASSUME: I can add participant to event before context.SaveChanges()

                    // Add pending to Event
                    thisPendingList.ForEach(p => NewEvent.Pending.Add(p));


                    if (thisParticipantList.Count >= 1)
                    {

                        // Copy RandomCommentText
                        List<string> currentCommTextList = new List<string>(RandomCommentText);
                        // Create thisCommentList
                        List<Comment> thisCommentList = new List<Comment>();


                        // Randomized #number# of comments
                        //int numberOfComments = rnd.Next(0, thisParticipantList.Count);
                        int numberOfComments = thisParticipantList.Count < 6 ? thisParticipantList.Count * 2 : thisParticipantList.Count;
                        // Create random comments
                        for (int j = 0; j < numberOfComments; j++)
                        {
                            // Get random textIndex
                            int currentCommTextIndex = rnd.Next(0, currentCommTextList.Count);

                            var comment = new Comment // ***** Will it be OK without ID? Save changes? *****
                            {
                                // Get random participant                    
                                //PosterId = thisParticipantList[rnd.Next(0, thisParticipantList.Count)].Id, // Or use EventParticipant list? *****ATM: Creator can't post******
                                PosterId = NewEvent.Participants.ToList()[rnd.Next(0, NewEvent.Participants.Count)].Id, // ****SO TRY THIS****
                                CommentText = currentCommTextList[currentCommTextIndex],
                                PostedTime = DateTime.Today.AddMinutes(-rnd.Next(0, 24 * 60 * 10)) // Last 10 days
                            };

                            // Remove text from currentRandomCommText
                            // currentCommTextList.RemoveAt(currentCommTextIndex);
                            // Add comment to thisCommentList
                            thisCommentList.Add(comment);
                        }
                        // Add comments to event *****FIRST TIME THE COMMENTS ARE ASSOSCIATED W/EVENT*****
                        thisCommentList.ForEach(c => NewEvent.Comments.Add(c));
                    }
                    // Save changes => I saved changes twice for each event in loop
                    context.SaveChanges();

                } // Create events loop - END

                context.Events.AddOrUpdate(NewReferenceEvent1);

                context.SaveChanges();
            } // IF !REFERNNCE_EVENT - END

        }
    }
}
//context.Users.AddOrUpdate(
//    p => p.UserName,
//    new ApplicationUser { UserName = "SuperAdmin", Email = "mariusnitt@hotmail.com", DateOfBirth = new DateTime(1990, 11, 8), Gender = "male" }
//    );

//context.SaveChanges(); // Else: NullReferenceException on addToRoles

//// Add SuperAdmin to roles
//var adminUser = manager.FindByName("SuperAdmin");
//manager.AddToRoles(adminUser.Id, new string[] { "SuperAdmin", "Admin" });

//context.SaveChanges();
