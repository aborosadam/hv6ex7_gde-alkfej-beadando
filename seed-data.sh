#!/bin/bash
BASE="http://movies.localhost:8088"

add_movie() {
  local title="$1"
  local director="$2"
  local year="$3"
  local genre="$4"
  local description="$5"
  
  response=$(curl -s -X POST "$BASE/api/Movies" \
    -H "Content-Type: application/json" \
    -d "{\"title\":\"$title\",\"director\":\"$director\",\"year\":$year,\"genre\":\"$genre\",\"description\":\"$description\",\"posterUrl\":\"\"}")
  echo "$response" | python3 -c "import json,sys; print(json.load(sys.stdin)['id'])"
}

add_review() {
  local movie_id="$1"
  local user="$2"
  local rating="$3"
  local comment="$4"
  
  curl -s -X POST "$BASE/api/Reviews" \
    -H "Content-Type: application/json" \
    -d "{\"movieId\":\"$movie_id\",\"userName\":\"$user\",\"rating\":$rating,\"comment\":\"$comment\"}" > /dev/null
}

echo "Adding movies..."

ID1=$(add_movie "Inception" "Christopher Nolan" 2010 "Sci-Fi" "A thief who steals corporate secrets through dream-sharing technology.")
add_review "$ID1" "Alice" 5 "Mind-bending masterpiece."
add_review "$ID1" "Bob" 4 "Great visuals."
add_review "$ID1" "Charlie" 5 "Nolan at his peak."

ID2=$(add_movie "The Matrix" "Wachowskis" 1999 "Sci-Fi" "A hacker discovers the true nature of reality.")
add_review "$ID2" "Diana" 5 "Revolutionary film."
add_review "$ID2" "Eve" 4 "Holds up well."

ID3=$(add_movie "Pulp Fiction" "Quentin Tarantino" 1994 "Crime" "Lives of mob hitmen, a boxer, and others intertwine.")
add_review "$ID3" "Frank" 5 "A perfect script."
add_review "$ID3" "Grace" 4 "Tarantino best work."
add_review "$ID3" "Henry" 5 "Royale with cheese."

ID4=$(add_movie "Goodfellas" "Martin Scorsese" 1990 "Crime" "The story of Henry Hill in the mob.")
add_review "$ID4" "Iris" 5 "Classic Scorsese."
add_review "$ID4" "Jack" 4 "Great film."

ID5=$(add_movie "Fight Club" "David Fincher" 1999 "Drama" "An insomniac office worker forms an underground fight club.")
add_review "$ID5" "Kate" 4 "First rule of fight club."
add_review "$ID5" "Leo" 3 "Overrated but interesting."

ID6=$(add_movie "Interstellar" "Christopher Nolan" 2014 "Sci-Fi" "Explorers travel through a wormhole in space.")
add_review "$ID6" "Mia" 5 "Tears every time."
add_review "$ID6" "Nick" 5 "Best space movie."
add_review "$ID6" "Olive" 4 "Beautiful but plot has holes."

ID7=$(add_movie "The Godfather" "Francis Ford Coppola" 1972 "Crime" "The aging patriarch of a crime dynasty transfers control.")
add_review "$ID7" "Paul" 5 "Cinematic masterpiece."
add_review "$ID7" "Quinn" 5 "Greatest film ever made."

ID8=$(add_movie "Parasite" "Bong Joon-ho" 2019 "Thriller" "A poor family schemes to become employed by a wealthy family.")
add_review "$ID8" "Rosa" 5 "Best Picture deserved."
add_review "$ID8" "Sam" 4 "Wild ride."

ID9=$(add_movie "The Dark Knight" "Christopher Nolan" 2008 "Action" "Batman faces the Joker, a criminal mastermind.")
add_review "$ID9" "Tina" 5 "Heath Ledger Joker is iconic."
add_review "$ID9" "Uma" 5 "Why so serious?"

ID10=$(add_movie "Forrest Gump" "Robert Zemeckis" 1994 "Drama" "The story of a slow-witted but kind man through history.")
add_review "$ID10" "Vince" 4 "Life is like a box of chocolates."
add_review "$ID10" "Wendy" 5 "Run Forrest, run!"

ID11=$(add_movie "Spirited Away" "Hayao Miyazaki" 2001 "Animation" "A girl wanders into a world ruled by gods and witches.")
add_review "$ID11" "Xander" 5 "Studio Ghibli at its best."

ID12=$(add_movie "The Shawshank Redemption" "Frank Darabont" 1994 "Drama" "Two imprisoned men bond over years and find redemption.")
add_review "$ID12" "Yara" 5 "Hope is a good thing."
add_review "$ID12" "Zack" 5 "Get busy living."

echo ""
echo "Done! Added 12 movies with 25 reviews."