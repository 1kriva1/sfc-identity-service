ECHO "Start creating User JWTS!"
dotnet user-jwts create --name "b0788d2f-8003-43c1-92a4-edc76a7c5dde" --audience "sfc.generaltemplate" --issuer "https://localhost:7266" --scope "sfc.generaltemplate.full" --scope "profile" --scope "openid" --scope "offline_access" --valid-for 365d --project "src/API/SFC.GeneralTemplate.Api/SFC.GeneralTemplate.Api.csproj"
ECHO "User JWTS creation finished!"
ECHO "Solution successfully created!"