#!/bin/bash
# remove-duplicate-posts.sh
# Removes duplicate Markdown posts (ignoring date, case, and punctuation)
# Keeps the most recent version based on filename date
# Moves duplicates to ./duplicates for review

set -e

POST_DIR="./"
DUP_DIR="./duplicates"

mkdir -p "$DUP_DIR"

echo "Scanning for duplicates in $POST_DIR ..."

# Create a temp file for normalized name mapping
TMPFILE=$(mktemp)

# Normalize each filename and store key + path
for file in "$POST_DIR"/*.md; do
  [ -f "$file" ] || continue
  base=$(basename "$file")
  date_part="${base:0:10}"
  title_part="${base:11}"

  # Normalize title: lowercase, remove punctuation
  key=$(echo "$title_part" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9]/-/g' | sed 's/-\+/-/g' | sed 's/-$//')
  echo "$key|$date_part|$file" >> "$TMPFILE"
done

# Sort by normalized title and date (descending)
sort -t"|" -k1,1 -k2,2r "$TMPFILE" | awk -F"|" '
BEGIN { prevkey=""; first=1; }
{
  key=$1; date=$2; file=$3;
  if (key == prevkey) {
    print "Duplicate found: " file;
    system("mv \"" file "\" ./duplicates/");
  } else {
    prevkey=key;
  }
}
END { print "Done! Duplicates moved to ./duplicates/" }
'

rm "$TMPFILE"
