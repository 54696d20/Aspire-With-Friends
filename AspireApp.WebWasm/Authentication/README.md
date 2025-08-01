# AuthBody Component

The `AuthBody` component is a reusable authentication wrapper that provides consistent authentication logic across all pages in the application.

## Features

- **Automatic Authentication Check**: Wraps content with `AuthorizeView` to ensure only authenticated users can access the content
- **Admin Role Check**: Optionally requires admin privileges (`godmode` role)
- **Consistent UI**: Provides a standardized "Access Denied" page for unauthorized users
- **User Information Access**: Provides user information to child components via cascading parameters
- **Customizable Title**: Optional page title display

## Usage

### Basic Usage

```razor
<AuthBody Title="My Protected Page">
    <h1>This content is only visible to authenticated users</h1>
    <p>Hello @authContext.userName!</p>
</AuthBody>
```

### Accessing User Information

```razor
@code {
    [CascadingParameter]
    private AuthBody authContext { get; set; }
    
    // Access user information:
    // authContext.userName - User's display name
    // authContext.isAdmin - Whether user has admin privileges
}
```

### Complete Example

```razor
@page "/my-page"

<AuthBody Title="My Protected Page">
    <MudAlert Severity="Severity.Success" Class="mb-4">
        Welcome, @authContext.userName! You have access to this page.
    </MudAlert>
    
    <h1>Protected Content</h1>
    <p>This content is only visible to authenticated users.</p>
    
    @if (authContext.isAdmin)
    {
        <MudAlert Severity="Severity.Info">
            You have admin privileges!
        </MudAlert>
    }
</AuthBody>

@code {
    [CascadingParameter]
    private AuthBody authContext { get; set; }
}
```

## Parameters

- `Title` (string): Optional page title to display
- `ChildContent` (RenderFragment): The content to display for authenticated users
- `RequireAdmin` (bool): Whether to require admin privileges (default: true)

## Pages Using AuthBody

The following pages in the application use the `AuthBody` component:

1. **Home** (`/`) - Main dashboard with live location updates
2. **Counter** (`/counter`) - Simple counter demonstration
3. **Weather** (`/weather`) - Weather forecast display
4. **Users** (`/users`) - User management dashboard
5. **Security** (`/security`) - Security management dashboard

### Navigation Structure

- **Home** - Main application dashboard
- **Counter** - Basic functionality demo
- **Weather** - Data display demo
- **Administration**
  - **Users** - User management (admin features available)
  - **Security** - Security settings (admin features available)

## Benefits

1. **DRY Principle**: No need to repeat `AuthorizeView` logic on every page
2. **Consistency**: All pages have the same authentication behavior
3. **Maintainability**: Authentication logic is centralized in one component
4. **User Experience**: Consistent "Access Denied" page across the application
5. **Easy Access**: Simple access to user information via cascading parameters

## How It Works

1. The component wraps content with `AuthorizeView`
2. For authenticated users, it checks if they have admin privileges (if required)
3. If authorized, it renders the child content with a cascading value containing user information
4. If not authorized, it shows a standardized "Access Denied" page with a login button
5. Child components can access user information via the `[CascadingParameter]` attribute

## Admin Features

Pages that include admin-specific features:
- **Users**: Admin can manage roles, perform bulk actions, and view analytics
- **Security**: Admin can manage access control, view audit logs, and configure security policies

These features are conditionally displayed based on the user's admin status (`authContext.isAdmin`). 