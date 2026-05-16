#!/usr/bin/fish

# ==========================================
# DATA FORMATTING FUNCTIONS
# ==========================================

function capitalize_word -a word
    set -l first_char (string sub -l 1 $word | string upper)
    set -l rest (string sub -s 2 $word)
    echo "$first_char$rest"
end

function capitalize_str -a raw_name
    set -l words (string split " " $raw_name)
    set -l cap_words
    for w in $words
        set -a cap_words (capitalize_word $w)
    end
    echo "$cap_words"
end

function generate_username -a raw_name index
    set -l base_uname (string replace -a " " "" $raw_name | string lower)
    echo "$base_uname$index"
end

function generate_password -a raw_name
    set -l first_name (string split " " $raw_name)[1]
    # FIXED: Using our custom function instead of string title
    set -l cap_first_name (capitalize_word $first_name)
    echo "$cap_first_name"123
end

function generate_email -a username
    echo "$username@email.com"
end

# ==========================================
# API COMMUNICATION FUNCTION
# ==========================================

function send_to_api -a name username password email
    curl -s -X POST http://localhost:5206/api/user/register \
         -H "Content-Type: application/json" \
         -d "{
                \"name\": \"$name\",
                \"birthDate\": \"2000-01-01T00:00:00Z\",
                \"userName\": \"$username\",
                \"password\": \"$password\",
                \"email\": \"$email\"
              }" > /dev/null
end

# ==========================================
# MAIN EXECUTION FLOW
# ==========================================

function seed_database
    set -l names "hikaro" "pedro henrique" "gabriel terres" "pedro ivo" "lucas silva" "marina souza" "beatriz oliveira" "rodrigo costa" "aline nunes" "thiago pinto" "julia martins" "felipe alves" "carla dias"

    echo "Starting database seeding process..."
    echo "--------------------------------------------------"

    for i in (seq 1 (count $names))
        set -l raw_name $names[$i]

        set -l formatted_name (capitalize_str $raw_name)
        set -l username (generate_username $raw_name $i)
        set -l password (generate_password $raw_name)
        set -l email (generate_email $username)

        echo " -> Registering: $formatted_name | User: $username | Password: $password"

        send_to_api $formatted_name $username $password $email
    end

    echo "--------------------------------------------------"
    echo "Process completed successfully!"
end

seed_database
