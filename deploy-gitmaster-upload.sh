echo "Deploying to Master Source Code except Site..."

# Ensure you're in the project root
git checkout master || exit 1

# Check for untracked or modified files
git status

# Add changes interactively (optional)
# git add -p

# Add all changes
git add .

# Commit with a descriptive message
git commit -m "My commits"

# Push changes to programmingstudy remote
git push -u programmingstudy master

echo "Deployment completed!"

