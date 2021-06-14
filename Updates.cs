/*
------------------------------------------UPDATES TO THE THINGY-------------------------------------------

- Added ability to select and hover over items properly (debounce code sorta), and simplified my hand tracking code

- Added body identification in body view - The active user is green, all other tracked bodies are red

- Made it far more robust and flexible, a global function for select and hover of menu item's rather than seperatley etc (hopefully lol)
  Haven't been able to break it with multiple users .. yet ..

- Added Man raising hand animation on the welcome page (only took 2 hours and 50 lines of code)

- Replaced hand pointer circles with Hand images

- Passed the hand state variable from body view to hand pointer class and added open and closed hand state images

- EXIT/BACK button dynamically updates 

- Integrated Jason's database and SQL query, allows me to retrive data from his database

- Speech Recognition
    - Generated a list of speech commands
        - Directly added the menu commands
        - Wrote a list of speech term beginning's eg 'Where is', 'When is', 'What time is', 'How do I get to'
        - Added a column in the database with paper/room speech terms eg COMPX241 speech term "compx two four one"

    - Requested all papers and rooms from the database
    - For each of the papers and rooms I appended the appropriate speech term beginnings to the speech term and created a new speech term
    - Added all of the speech terms to a Grammer Object. This is used by the speech recognition library
    - The Speech Recognition engine takes what I say and compares it to each term in the grammer object, and generates a 
      rating for how likely it think it iw that you have said that particular if the result is above a certain threshold it means
      it thinks it has recognizeed what you've said
    - Made a Function for onRecognition
    - Realised I need to categorize search term so I created a class that contains the search term, a name and type. I created one of these objects
      in the same way as the search terms and added each to a List of search term objects
    - When a search term is recognized the result is compared to each element in the search term object until it is found, then gets the type of
      search term object it is
    - Does stuff based on the type eg. If the name of a menu item is said, it will show that menu item
    - Added ability to pause and start speech recognition. Took me forever to figure out how to pause it..... as is asynchronus and you cant 
      stop it during recognition
    
- Added help messages, using an asynchronus class that updates the message after a certain time and forever iterates through an array
    help messages.

- Commented ALL the code (Literally 2 hours of straight commenting lol)

- Fixed all Intellisense issues / Naming conventions so code be professional lol

/----------------------------------------------NEXT STEPS----------------------------------------------

- Easter Eggs hehehehe

- Integrate Jason's added methods in his SQL class
    - I should be give his method a paperCode and it should return the location / time of the next time for that paper

- Integrate Mal's Map system which will take a room which is returned from my onRecognition function, 
  and give me a image of a map, and direction as a string

//-----------------------------------------------------------------------------------------------------
*/
