echo "Deploying to Master Source Code except Site..."

# Ensure you're in the project root
git checkout master || exit 1

# Check for untracked or modified files
git status

# Add changes interactively (optional)
# git add -p

# Add changes
git add .

# Prompt for a commit message
echo "Enter your commit message:"
read commit_message

# Check if a message was entered
if [ -z "$commit_message" ]; then
  echo "No commit message entered. Exiting."
  exit 1
fi

# Commit with the provided message
git commit -m "$commit_message"

# Push changes to programmingstudy remote
git push -u programmingstudy master

echo "Deployment completed!"

