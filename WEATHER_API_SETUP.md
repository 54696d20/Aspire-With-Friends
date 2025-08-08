# Weather API Setup Guide

This project uses [WeatherAPI.com](https://www.weatherapi.com/) to provide real weather data. Follow these steps to configure your API key securely.

## 🔑 Getting Your API Key

1. Visit [WeatherAPI.com](https://www.weatherapi.com/)
2. Sign up for a free account
3. Get your API key from the dashboard
4. The free tier includes:
   - 1,000,000 calls per month
   - Current weather, forecasts, and location search
   - Real-time weather data

## 🛡️ Secure Configuration

### Option 1: .NET User Secrets (Recommended for Development)

User secrets are stored locally and never committed to git.

```bash
# Navigate to the WeatherAPI project
cd AspireApp.WeatherAPI

# Initialize user secrets (if not already done)
dotnet user-secrets init

# Add your API key
dotnet user-secrets set "WeatherApi:ApiKey" "YOUR_ACTUAL_API_KEY_HERE"
```

### Option 2: Environment Variables (For Production)

Set the environment variable `WEATHERAPI_KEY`:

```bash
# macOS/Linux
export WEATHERAPI_KEY="YOUR_ACTUAL_API_KEY_HERE"

# Windows
set WEATHERAPI_KEY=YOUR_ACTUAL_API_KEY_HERE
```

### Option 3: Azure Key Vault (For Production)

For production deployments, consider using Azure Key Vault or similar secure configuration management.

## 🚀 Testing Your Setup

1. Start your AppHost in Rider
2. Navigate to the Weather page in your WASM app
3. Search for any city (e.g., "London", "New York", "Tokyo")
4. You should see real weather data displayed

## 🔍 Troubleshooting

### "WeatherAPI key not configured" Error

If you see this error, check:

1. **User Secrets**: Verify your API key is set correctly
   ```bash
   dotnet user-secrets list
   ```

2. **Environment Variable**: Check if the environment variable is set
   ```bash
   echo $WEATHERAPI_KEY
   ```

3. **API Key Validity**: Test your API key directly
   ```bash
   curl "https://api.weatherapi.com/v1/current.json?key=YOUR_API_KEY&q=London"
   ```

### Rate Limiting

The free tier has limits:
- 1,000,000 calls per month
- 3 calls per second

If you hit limits, consider upgrading your plan.

## 📝 API Endpoints

The WeatherAPI service exposes these endpoints:

- `GET /api/weather/current/{query}` - Current weather
- `GET /api/weather/forecast/{query}?days=7` - 7-day forecast  
- `GET /api/weather/search/{query}` - Location search

## 🔒 Security Notes

- ✅ API keys are stored in user secrets (not in git)
- ✅ Environment variables are supported as fallback
- ✅ No sensitive data in configuration files
- ✅ .gitignore excludes sensitive files

## 📚 Additional Resources

- [WeatherAPI.com Documentation](https://www.weatherapi.com/docs/)
- [.NET User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Environment Variables in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration) 