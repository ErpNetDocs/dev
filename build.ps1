param(
    [string]$docfx,
    [string]$remote,
    [switch]$debug
)

if ($debug -eq $true) {
    #$git_bin = "C:\Program Files\Git\usr\bin"
    $working_directory = Get-Location | Out-String
    Write-Host "cwd is $working_directory"
    Write-Host "user is $env:USERNAME"

    if (test-path "$home\.ssh\id_rsa.pub") {
        Write-Host "Has public key:"
        Get-Content "$home\.ssh\id_rsa.pub"
    }
    else {
        Write-Host "Doesn't have SSH public key!!"
    }
}


Write-Host "Setting up git remote ssh..."

git config remote.ssh.url
if ($lastexitcode -eq 1) {
    git remote add ssh $remote
}

git checkout master
Write-Host "Undoing local changes..."
git reset --hard HEAD^

Write-Host "Building docfx"
Invoke-Expression "$docfx build"

Write-Host "Upload Changes to Github"
git add -A
git push ssh master
