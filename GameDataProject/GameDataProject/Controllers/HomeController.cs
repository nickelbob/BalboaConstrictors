﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using GameDataProject.Models;
using Microsoft.VisualBasic.FileIO;

namespace GameDataProject.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportJSONGameData()
        {
            ImportJSONGameDataModel model = new ImportJSONGameDataModel();

            model = PrepareImportJSONGameData(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult ImportJSONGameData(ImportJSONGameDataModel model, int games)
        {
            model = PrepareImportJSONGameData(model);

            try
            {
                // Verify that the user selected a file
                if (model.file != null && model.file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(model.file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    model.file.SaveAs(path);

                    if(model.importJSON)
                        ImportJSONGameDataFile(path, games);
                    else
                        ImportCSVGameDataFile(path, games);
                }

                model.Status = "Import Succeeded!";
            }
            catch (Exception ex)
            {
                model.Status = ex.Message + " " + ex.StackTrace;
            }

            return View(model);
        }

        private void ImportCSVGameDataFile(string path, int gameId)
        {
            // TextFieldParser is in the Microsoft.VisualBasic.FileIO namespace.
            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.CommentTokens = new string[] { "#" };
                parser.SetDelimiters(new string[] { "," });
                parser.HasFieldsEnclosedInQuotes = false;

                // Skip over header line.
                //parser.ReadLine();
                List<string> header = parser.ReadFields().ToList();
                List<GameData> gameData = new List<GameData>();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    DateTime toe = new DateTime();
                    if (!DateTime.TryParse(fields[header.IndexOf("when")], out toe))
                    {
                        using (BalboaConstrictorsEntities e = new BalboaConstrictorsEntities())
                        {
                            string s = e.Games.FirstOrDefault(g => g.Id == gameId).BeginTime.Date.ToShortDateString() + " " + fields[header.IndexOf("when")];
                            s = s.Insert(s.LastIndexOf(':'), ".").Remove(s.LastIndexOf(':') + 1, 1);
                            toe = Convert.ToDateTime(s);
                            //toe = Convert.ToDateTime(s);
                        }
                    }

                    gameData.Add(new GameData()
                    {
                        who = fields[header.IndexOf("who")],
                        what = fields[header.IndexOf("what")],
                        when = toe,
                        team = fields[header.IndexOf("team")]
                    });
                }

                gameData = gameData.OrderBy(gd => gd.when).ThenBy(gd => gd.what).ToList();

                for (int i = 0; i < gameData.Count() - 1; i++)
                {
                    if (gameData[i].what == "Completed Pass" && gameData[i + 1].what == "Reception")
                    {
                        EnterCompletedPass(gameId, gameData[i].who, gameData[i + 1].who, gameData[i].when, gameData[i].team);
                        i++;
                        continue;
                    }
                    else if (gameData[i].what == "Turnover" && gameData[i+1].what == "Block")
                    {
                        EnterBlockedPass(gameId, gameData[i].who, gameData[i + 1].who, gameData[i].when, gameData[i].team);
                        i++;
                        continue;
                    }
                    else if (gameData[i].what == "Turnover" && gameData[i + 1].what != "Block")
                    {
                        EnterIncompletePass(gameId, gameData[i].who, gameData[i].when, gameData[i].team);
                        continue;
                    }
                }
            }
        }

        private void EnterIncompletePass(int GameId, string Passer, DateTime TimeOfEvent, string teamName)
        {
            using (BalboaConstrictorsEntities entity = new BalboaConstrictorsEntities())
            {
                Player passPly = entity.Players.FirstOrDefault(p => p.FirstName == Passer);

                if (passPly == null)
                {
                    passPly = entity.Players.Add(new Player()
                    {
                        FirstName = Passer,
                        LastName = ""
                    });
                }

                PlayerTeam plyTeam = entity.PlayerTeams.FirstOrDefault(pt => pt.PlayerId == passPly.Id && pt.Team.TeamName == teamName);
                if (plyTeam == null)
                {
                    var team = entity.Teams.FirstOrDefault(t => t.TeamName == teamName);

                    if (team == null)
                    {
                        team = entity.Teams.Add(new Team()
                        {
                            TeamName = teamName
                        });
                    }

                    plyTeam = entity.PlayerTeams.Add(new PlayerTeam()
                    {
                        Player = passPly,
                        Team = team
                    });
                }

                EventType incompletedPass = entity.EventTypes.FirstOrDefault(e => e.EventTypeName == "Incomplete Pass");

                if (incompletedPass == null)
                {
                    incompletedPass = entity.EventTypes.Add(new EventType()
                    {
                        EventTypeName = "Incomplete Pass"
                    });
                }


                EventTypeParticipant passPart = entity.EventTypeParticipants.FirstOrDefault(p => p.ParticipantLabel == "Passer" && p.EventTypeId == incompletedPass.Id);

                var events = entity.Events.Where(e => e.GameId == GameId && e.TimeOfEvent == TimeOfEvent).ToList();


                if (events.Where(e => e.EventType == incompletedPass).Count() == 0)
                {

                    Event evt = entity.Events.Add(new Event()
                    {
                        GameId = GameId,
                        TimeOfEvent = TimeOfEvent,
                        EventType = incompletedPass 
                    });

                    entity.EventPlayers.Add(new EventPlayer()
                    {
                        Event = evt,
                        Player = passPly,
                        EventTypeParticipant = passPart
                    });

                    entity.SaveChanges();
                }
            }
        }

        private void EnterBlockedPass(int GameId, string Passer, string Blocker, DateTime TimeOfEvent, string teamName)
        {
            using (BalboaConstrictorsEntities entity = new BalboaConstrictorsEntities())
            {
                Player passPly = entity.Players.FirstOrDefault(p => p.FirstName == Passer);

                if (passPly == null)
                {
                    passPly = entity.Players.Add(new Player()
                    {
                        FirstName = Passer,
                        LastName = ""
                    });
                }

                Player blockPly = entity.Players.FirstOrDefault(p => p.FirstName == Blocker);

                if (blockPly == null)
                {
                    blockPly = entity.Players.Add(new Player()
                    {
                        FirstName = Blocker,
                        LastName = ""
                    });
                }

                PlayerTeam plyTeam = entity.PlayerTeams.FirstOrDefault(pt => pt.PlayerId == passPly.Id && pt.Team.TeamName == teamName);
                if (plyTeam == null)
                {
                    var team = entity.Teams.FirstOrDefault(t => t.TeamName == teamName);

                    if (team == null)
                    {
                        team = entity.Teams.Add(new Team()
                        {
                            TeamName = teamName
                        });
                    }

                    plyTeam = entity.PlayerTeams.Add(new PlayerTeam()
                    {
                        Player = passPly,
                        Team = team
                    });
                }

                EventType block = entity.EventTypes.FirstOrDefault(e => e.EventTypeName == "Block");

                if (block == null)
                {
                    block = entity.EventTypes.Add(new EventType()
                    {
                        EventTypeName = "Block"
                    });
                }

                EventTypeParticipant passPart = entity.EventTypeParticipants.FirstOrDefault(p => p.ParticipantLabel == "Passer" && p.EventTypeId == block.Id);
                EventTypeParticipant blockPart = entity.EventTypeParticipants.FirstOrDefault(p => p.ParticipantLabel == "Blocker" && p.EventTypeId == block.Id);

                var events = entity.Events.Where(e => e.GameId == GameId && e.TimeOfEvent == TimeOfEvent).ToList();


                if (events.Where(e => e.EventType == block).Count() == 0)
                {

                    Event evt = entity.Events.Add(new Event()
                    {
                        GameId = GameId,
                        TimeOfEvent = TimeOfEvent,
                        EventType = block
                    });

                    entity.EventPlayers.Add(new EventPlayer()
                    {
                        Event = evt,
                        Player = passPly,
                        EventTypeParticipant = passPart
                    });

                    entity.EventPlayers.Add(new EventPlayer()
                    {
                        Event = evt,
                        Player = blockPly,
                        EventTypeParticipant = blockPart
                    });

                    entity.SaveChanges();
                }
            }
        }

        private void ImportJSONGameDataFile(string path, int gameId)
        {
            string JSONText = String.Empty;

            if (System.IO.File.Exists(path))
            {
                JSONText = String.Join("", System.IO.File.ReadAllLines(path));
            }

            //Going to need the events "Completed Pass", "Block", and "Point"

            JavaScriptSerializer jss = new JavaScriptSerializer();

            List<GameData> gameData = (List<GameData>)(jss.Deserialize(JSONText, typeof(List<GameData>)));

            gameData = gameData.OrderBy(gd => gd.when).ThenBy(gd=>gd.what).ToList();

            for (int i = 0; i < gameData.Count(); i++)
            {
                if (gameData[i].what == "Completed Pass" && gameData[i+1].what == "Reception")
                {
                    EnterCompletedPass(gameId, gameData[i].who, gameData[i + 1].who, gameData[i].when, "Light");
                    i++;
                    continue;
                }
            }

        }

        private void EnterCompletedPass(int GameId, string Passer, string Catcher, DateTime TimeOfEvent, string teamName)
        {
            using (BalboaConstrictorsEntities entity = new BalboaConstrictorsEntities())
            {
                Player passPly = entity.Players.FirstOrDefault(p => p.FirstName == Passer);

                if (passPly == null)
                {
                    passPly = entity.Players.Add(new Player()
                    {
                        FirstName = Passer,
                        LastName = ""
                    });
                }

                Player catchPly = entity.Players.FirstOrDefault(p => p.FirstName == Catcher);

                if (catchPly == null)
                {
                    catchPly = entity.Players.Add(new Player()
                    {
                        FirstName = Catcher,
                        LastName = ""
                    });
                }

                PlayerTeam plyTeam = entity.PlayerTeams.FirstOrDefault(pt => pt.PlayerId == passPly.Id && pt.Team.TeamName == teamName);
                if (plyTeam == null)
                {
                    var team = entity.Teams.FirstOrDefault(t=>t.TeamName == teamName);

                    if(team == null)
                    {
                        team = entity.Teams.Add(new Team()
                        {
                             TeamName = teamName
                        });
                    }

                    plyTeam = entity.PlayerTeams.Add(new PlayerTeam()
                    {
                        Player = passPly,
                         Team = team
                    });
                }

                EventType completedPass = entity.EventTypes.FirstOrDefault(e => e.EventTypeName == "Completed Pass");

                if (completedPass == null)
                {
                    completedPass = entity.EventTypes.Add(new EventType()
                    {
                        EventTypeName = "Completed Pass"
                    });
                }

                entity.SaveChanges();

                EventTypeParticipant passPart = entity.EventTypeParticipants.FirstOrDefault(p => p.ParticipantLabel == "Passer" && p.EventTypeId == completedPass.Id);
                EventTypeParticipant catchPart = entity.EventTypeParticipants.FirstOrDefault(p => p.ParticipantLabel == "Catcher" && p.EventTypeId == completedPass.Id);

                entity.SaveChanges();

                var events = entity.Events.Where(e => e.GameId == GameId && e.TimeOfEvent == TimeOfEvent).ToList();
                

                if (events.Where(e=> e.EventType == completedPass).Count() == 0)
                {

                    Event evt = entity.Events.Add(new Event()
                    {
                        GameId = GameId,
                        TimeOfEvent = TimeOfEvent,
                        EventType = completedPass
                    });

                    entity.EventPlayers.Add(new EventPlayer()
                    {
                        Event = evt,
                        Player = passPly,
                        EventTypeParticipant = passPart
                    });

                    entity.EventPlayers.Add(new EventPlayer()
                    {
                        Event = evt,
                        Player = catchPly,
                        EventTypeParticipant = catchPart
                    });

                    entity.SaveChanges();
                }
            }
        }

        private ImportJSONGameDataModel PrepareImportJSONGameData(ImportJSONGameDataModel model)
        {
            if(model == null)
                model = new ImportJSONGameDataModel();

            using (BalboaConstrictorsEntities entity = new BalboaConstrictorsEntities())
            {
                model.games = entity.Games.ToList();


                foreach (var p in model.games)
                {
                    model.gameSelect.Add(new SelectListItem()
                    {
                        Text = String.Join(" vs. ", p.GameTeams.Select(gt => gt.Team.TeamName).ToArray()) + " @ " + p.BeginTime.ToString("F"),
                        Value = p.Id.ToString()
                    });
                }
            }

            return model;
        }

        public ActionResult ImportKML()
        {

            ImportKMLModel model = new ImportKMLModel();
            model = PrepareImportKMLModel(model);

            return View(model);
        }

        private ImportKMLModel PrepareImportKMLModel(ImportKMLModel model)
        {
            model = new ImportKMLModel();
            using (BalboaConstrictorsEntities entity = new BalboaConstrictorsEntities())
            {
                model.players = entity.Players.ToList();
                model.dataTypes = entity.DataTypes.ToList();
            }

            foreach (var dt in model.dataTypes)
            {
                model.dataTypeSelect.Add(new SelectListItem()
                {
                    Text = dt.DataType1,
                    Value = dt.Id.ToString()
                });
            }

            foreach (var p in model.players)
            {
                model.playersSelect.Add(new SelectListItem()
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.Id.ToString()
                });
            }

            return model;
        }

        [HttpPost]
        public ActionResult ImportKML(ImportKMLModel model, int players, int dataTypes)
        {
            try
            {
                // Verify that the user selected a file
                if (model.file != null && model.file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(model.file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    model.file.SaveAs(path);

                    ImportKMLFile(path, players, dataTypes);
                }

                model = PrepareImportKMLModel(model);

                model.Status = "Import Succeeded!";
            }
            catch (Exception ex)
            {
                model.Status = ex.Message + " " + ex.StackTrace;
            }

            return View(model);
        }

        private void ImportKMLFile(string path, int id, int dataType)
        {
            //int playerId = -1;

            //if (!Int32.TryParse(id, out playerId))
            //    return;

            DataType dt = null;
            using (BalboaConstrictorsEntities entity = new BalboaConstrictorsEntities())
            {
                dt = entity.DataTypes.Where(d => d.Id == dataType).FirstOrDefault();

                if (dt == null)
                {
                    return;
                    //dt = new DataType();
                    //dt.DataType1 = "GPS";
                    //entity.DataTypes.Add(dt);
                    //entity.SaveChanges();
                }

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    XDocument feedXml = XDocument.Load(fs);
                    var feeds = feedXml.Descendants("Placemark");

                    feeds = feeds.Where(x => x.HasAttributes);

                    var feedList = feedXml.Document.Descendants().ToList();
                    var Placemark = feedList.Where(x => x.Name.LocalName.Contains("Placemark") && x.HasAttributes).Descendants().ToList();

                    for (int i = 0; i < Placemark.Count() - 1; i++)
                    {
                        if (Placemark[i].Name.LocalName.Contains("when") && Placemark[i + 1].Name.LocalName.Contains("coord"))
                        {
                            DateTime TOE = new DateTime();
                            if (DateTime.TryParse(Placemark[i].Value, out TOE))
                            {
                                //First check that we haven't already added this.
                                //Check for playerID and timestamp
                                if (!entity.PlayerDatas.Any(p => p.PlayerId == id && p.TimeOfData == TOE))
                                {
                                    PlayerData pd = new PlayerData();
                                    pd.DataType = dt;
                                    pd.TimeOfData = TOE;
                                    pd.TextData = Placemark[i + 1].Value;
                                    pd.PlayerId = id;
                                    entity.PlayerDatas.Add(pd);
                                    entity.SaveChanges();
                                }
                            }

                            //Skip the next since we know it's a coord
                            i++;
                        }
                    }

                }
            }
        }

        public ActionResult HeatMap1()
        {
            HeatMap1Model model = new HeatMap1Model();

            string[] arr;
            GPSCoord g;
            List<GPSCoord> tempList;
            MinMaxObj minMax;

            using(BalboaConstrictorsEntities e = new BalboaConstrictorsEntities())
            {
                foreach (var game in e.Games)
                {
                    var GPSCoords = e.PlayerDatas.Where(pd => pd.DataType.DataType1 == "GPS" && pd.TimeOfData > game.BeginTime && pd.TimeOfData < game.EndTime);

                    minMax = new MinMaxObj();
                    tempList = new List<GPSCoord>();

                    foreach (var pd in GPSCoords)
                    {
                        arr = pd.TextData.Split(' ');

                        g = new GPSCoord()
                        {
                            x = Convert.ToDouble(arr[0]),
                            y = Convert.ToDouble(arr[1])
                        };



                        if (g.x > minMax.maxX)
                            minMax.maxX = g.x;
                        else if (g.x < minMax.minX)
                            minMax.minX = g.x;

                        if (g.y < minMax.minY)
                            minMax.minY = g.y;
                        else if (g.y > minMax.maxY)
                            minMax.maxY = g.y;

                        tempList.Add(g);

                    }

                    model.GPSData.Add(game.BeginTime.ToString("F"), tempList);

                    var BoundaryData = e.PlayerDatas.Where(pd => pd.DataType.DataType1 == "Boundary" && pd.TimeOfData > game.BeginTime && pd.TimeOfData < game.EndTime);
                    tempList = new List<GPSCoord>();
                    foreach (var pd in BoundaryData)
                    {
                        arr = pd.TextData.Split(' ');

                        g = new GPSCoord()
                        {
                            x = Convert.ToDouble(arr[0]),
                            y = Convert.ToDouble(arr[1])
                        };

                        if (g.x > minMax.maxX)
                            minMax.maxX = g.x;
                        else if (g.x < minMax.minX)
                            minMax.minX = g.x;

                        if (g.y < minMax.minY)
                            minMax.minY = g.y;
                        else if (g.y > minMax.maxY)
                            minMax.maxY = g.y;

                        tempList.Add(g);

                    }

                    model.BoundaryData.Add(game.BeginTime.ToString("F"), tempList);
                    model.minMax.Add(game.BeginTime.ToString("F"), minMax);
                }

            }
            return View(model);
        }

        public ActionResult PassingChordDiagram()
        {
            PassingChordDiagramModel model = new PassingChordDiagramModel();
            int i = 0;

            using (BalboaConstrictorsEntities e = new BalboaConstrictorsEntities())
            {
                //Get Names First
                Dictionary<string, int> posOfPlayers = new Dictionary<string,int>();
                var random = new Random();
                foreach (var p in e.Players)
                {
                    if (p.FirstName == "Anon")
                        continue;

                    model.nameColorLst.Add(new PlayerNameColor()
                    {
                        Name = p.FirstName + " " + p.LastName,
                        Color = String.Format("#{0:X6}", random.Next(0x1000000))
                    });

                    posOfPlayers.Add(p.FirstName + " " + p.LastName, model.nameColorLst.Count() - 1);
                }

                int numPlayers = model.nameColorLst.Count();
                int[][] tempPlayerPass = new int[numPlayers][];

                for (i = 0; i < numPlayers; i++)
                    tempPlayerPass[i] = new int[numPlayers];
                

                foreach (var evnt in e.Events.Where(evt => evt.EventType.EventTypeName == "Completed Pass"))
                {
                    Player passer = evnt.EventPlayers.Where(etp=>etp.EventTypeParticipant.ParticipantLabel == "Passer").First().Player;
                    Player catcher = evnt.EventPlayers.Where(etp=>etp.EventTypeParticipant.ParticipantLabel == "Catcher").First().Player;

                    if (passer.FirstName == "Anon" || catcher.FirstName == "Anon")
                        continue;

                    tempPlayerPass[posOfPlayers[passer.FirstName + " " + passer.LastName]][posOfPlayers[catcher.FirstName + " " + catcher.LastName]]++;
                }

                JavaScriptSerializer jss = new JavaScriptSerializer();

                model.playerPassingArray = jss.Serialize(tempPlayerPass);

                //string tmpArrStr = "


                //for (var i = 0; i < numPlayers; i++)
                //{
                //    for (var j = 0; j < numPlayers; j++)
                //    {

                //    }
                //}
            }

            return View(model);
        }

        public ActionResult UnderUtilizedForceDiagram()
        {
            UnderUtilizedPeopleModel model;
            UnderUtilGameData uugameData = new UnderUtilGameData();
            Random r = new Random();

            model = Session["uuPeopleModel"] as UnderUtilizedPeopleModel;

            if (model != null)
            {
                return View(model);
            }
            else
                model = new UnderUtilizedPeopleModel();

            using (BalboaConstrictorsEntities e = new BalboaConstrictorsEntities())
            {
                foreach (var game in e.Games)
                {
                    var gameName = game.BeginTime + " " + game.GameTeams.ElementAt(0).Team.TeamName + " vs. " + game.GameTeams.ElementAt(1).Team.TeamName;

                    model.underUtilGameData.Add(gameName, new Dictionary<string, UnderUtilGameData>());

                    foreach (var team in game.GameTeams)
                    {
                        model.underUtilGameData[gameName].Add(team.Team.TeamName, new UnderUtilGameData());

                        foreach (var player in team.Team.PlayerTeams)
                        {
                            var uuGD = model.underUtilGameData[gameName][team.Team.TeamName];

                            //Grab all game events involving ths player
                            var playerEvents = game.Events.Where(evt => evt.EventPlayers.Any(ep => ep.PlayerId == player.PlayerId));

                            //Grab number of completed passes
                            var completePasses = playerEvents.Where(pe => pe.EventType.EventTypeName == "Completed Pass");
                            //var completePass
                            int numCompletedPasses = completePasses.Where(pe => pe.EventPlayers.Any(ep => ep.EventTypeParticipant.ParticipantLabel == "Passer" && ep.PlayerId == player.Player.Id)).Count();

                            if (numCompletedPasses == 0)
                                continue;

                            int numIncompletedPasses = playerEvents.Where(pe => pe.EventType.EventTypeName == "Incomplete Pass" && pe.EventPlayers.Any(ep => ep.EventTypeParticipant.ParticipantLabel == "Passer" && ep.PlayerId == player.Player.Id)).Count();

                            var catches = playerEvents.Where(pe => pe.EventType.EventTypeName == "Completed Pass" && pe.EventPlayers.Any(ep => ep.EventTypeParticipant.ParticipantLabel == "Catcher" && ep.PlayerId == player.Player.Id)).Count();

                            uuGD.Names.Add(new PlayerInfo()
                            {
                                compRate =  (numCompletedPasses + numIncompletedPasses == 0 ? 0 : numCompletedPasses / ((float)numCompletedPasses + numIncompletedPasses)),
                                Name = player.Player.FirstName + " " + player.Player.LastName,
                                numThrowsCatches = (int)(numCompletedPasses + catches)
                            });
                        }
                    }

                    var eventsInGame = e.Events.Where(evt=>evt.GameId == game.Id); 
                    
                    foreach(var eig in eventsInGame)
                    {
                        if (eig.EventType.EventTypeName == "Completed Pass")
                        {
                            var passer = eig.EventPlayers.FirstOrDefault(ep => ep.EventTypeParticipant.ParticipantLabel == "Passer");
                            var catcher = eig.EventPlayers.FirstOrDefault(ep => ep.EventTypeParticipant.ParticipantLabel == "Catcher");

                            string teamName = e.Teams.ToList().ElementAt(0).TeamName;
                            var findTeam = game.GameTeams.FirstOrDefault(gt => gt.Team.PlayerTeams.Any(pt => pt.Player.FirstName.Contains(passer.Player.FirstName) || passer.Player.FirstName.Contains(pt.Player.FirstName)));
                            if (findTeam != null)
                                teamName = findTeam.Team.TeamName;

                            var team = model.underUtilGameData[gameName][teamName];

                            //Find passer and catcher inside our team array
                            int passerPos = team.Names.FindIndex(pi=>pi.Name == passer.Player.FirstName + " " + passer.Player.LastName);
                            int catcherPos = team.Names.FindIndex(pi => pi.Name == catcher.Player.FirstName + " " + catcher.Player.LastName);

                            var pass1 = team.Passes.FirstOrDefault(p => p.source == passerPos && p.target == catcherPos);
                            var pass2 = team.Passes.FirstOrDefault(p => p.target == passerPos && p.source == catcherPos);

                            if (pass1 != null)
                            {
                                pass1.numThrows++;
                                pass1.throwDist = (pass1.throwDist + r.Next(5, 50)) / 2;
                            }
                            else if (pass2 != null)
                            {
                                pass2.numThrows++;
                                pass2.throwDist = (pass2.throwDist + r.Next(5, 50)) / 2;
                            }
                            else
                            {
                                if (passerPos > -1 && catcherPos > -1)
                                {
                                    team.Passes.Add(new PassInfo()
                                    {
                                        numThrows = 1,
                                        source = passerPos,
                                        target = catcherPos,
                                        throwDist = r.Next(5, 50)
                                    });
                                }
                            }

                        }
                    }
                }

                //foreach (var evt in e.Events)
                //{
                //    if(evt.EventType.EventTypeName != "Completed Pass")
                //        continue;

                //    var gameName = evt.Game.BeginTime + " " + evt.Game.GameTeams.ElementAt(0).Team.TeamName + " vs. " + evt.Game.GameTeams.ElementAt(1).Team.TeamName;
                //    var teamName = evt.
                //}
            }

            Session["uuPeopleModel"] = model;

            return View("UnderUtilizedForceDiagram", model);
        }
    }

    public class GameData
    {
        public string who { get; set; }
        public DateTime when { get; set; }
        public string what { get; set; }
        public string team { get; set; }
    }
}
