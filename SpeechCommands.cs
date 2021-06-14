using System;
using System.Collections.Generic;
using Microsoft.Speech.Recognition;
using compx241GroupProject;
namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    class SpeechCommands
    {
        readonly String[] where = { "How do I get to", "Where is", "How do I find" };
        readonly String[] when = { "When is", "What time is" };

        public class term
        {
            public String name, speech, p_name;
            public int type;
        }

        //TYPES//
        //0 = Global Menu Item      
        //1 = Asking map for room location directions (map)
        //2 = Asking map for paper location (map)
        //3 = Asking for paper time (calender)
        //4 = Asking for paper details / schedule (calender)
        //5 = lone paper code
        //6 = Map Menu Item

        //Possible things the user can do
        /*
         * Ask for a room location
         * Ask for block location 
         * Ask for a paper location (Where is paper)
         * Ask for a paper time (when is paper)
         * Ask for paper schedule/details 
         * Say menu icons
         */

        List<term> term_objects = new List<term>();

        public List<term> get_term_objects()
        {
            return term_objects;
        }

        public Choices get_search_terms()
        {
            //Standard Room Locations with their type
            term map = new term
            {
                name = "MAP",
                speech = "map",
                type = 0
            };
            term cal = new term
            {
                name = "CALENDAR",
                speech = "calendar",
                type = 0
            };
            term not = new term
            {
                name = "NOTICES",
                speech = "notices",
                type = 0
            };
            term back = new term
            {
                name = "BACK",
                speech = "back",
                type = 0
            };
            term exit = new term
            {
                name = "EXIT",
                speech = "exit",
                type = 0
            };
            term rl = new term
            {
                name = "RL",
                speech = "Room Location",
                type = 6
            };
            term pl = new term
            {
                name = "PL",
                speech = "Paper Location",
                type = 6
            };
            term tul = new term
            {
                name = "TUTORIAL",
                speech = "tutorial",
                type = 0
            };
            term game = new term
            {
                name = "GAME",
                speech = "game",
                type = 0
            };

            term_objects.Add(map);
            term_objects.Add(cal);
            term_objects.Add(not);
            term_objects.Add(back);
            term_objects.Add(exit);
            term_objects.Add(pl);
            term_objects.Add(rl);
            term_objects.Add(game);
            term_objects.Add(tul);

            Choices terms = new Choices();

            foreach (term t in term_objects)
            {
                terms.Add(new SemanticResultValue(t.speech, t.name));
            }           

            //Select all papers and thier speech codes
            string paperQuery = "SELECT paperCode, speech FROM Paper";
            SQL.selectQuery(paperQuery);
            while (SQL.read.Read())
            {
                //For each paper
                String paper_name = SQL.read[0].ToString();
                String paper_speech = SQL.read[1].ToString();

                Console.WriteLine("Paper : " + paper_name + " " + paper_speech);

                term paper = new term
                {
                    type = 5,
                    speech = paper_speech,
                    name = paper_name
                };
                term_objects.Add(paper);
                terms.Add(new SemanticResultValue(paper.speech, paper.name));


                foreach (String w in where)
                {
                    String where_speech = w + " " + paper_speech;
                    term where = new term
                    {
                        type = 2,
                        speech = where_speech,
                        name = where_speech,
                        p_name = paper_name
                    };

                    terms.Add(new SemanticResultValue(where.speech, where.speech));
                    term_objects.Add(where);
                }
                foreach (String w in when)
                {
                    String when_speech = w + " " + paper_speech;
                    term when = new term
                    {
                        type = 3,
                        speech = when_speech,
                        name = when_speech,
                        p_name = paper_name
                    };

                    terms.Add(new SemanticResultValue(when.speech, when.speech));
                    term_objects.Add(when);
                }
            }

            //Select all rooms and thier speech codes
            string roomQuery = "SELECT * FROM Room";
            SQL.selectQuery(roomQuery);
            while (SQL.read.Read())
            {
                //For each paper
                string room_speech = SQL.read[3].ToString();
                string room_name = SQL.read[0].ToString() + "." + SQL.read[1].ToString() + "." + SQL.read[2].ToString();
                foreach (String w in where)
                {
                    term room = new term
                    {
                        type = 1,
                        speech = w + " " + room_speech,
                        name = room_name
                    };
                    terms.Add(new SemanticResultValue(room.speech, room.name));
                    term_objects.Add(room);
                }
            }
            return terms;
        }
    }
}
