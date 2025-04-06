(function waitForSwaggerUI() {
  if (!window.ui?.authActions) {
    console.log('Waiting Swagger UI...');
    setTimeout(waitForSwaggerUI, 500);
    return;
  }

  console.log('Swagger UI Loaded!');

  let refreshTokenInterval = null;
  function safeClearInterval() {
    if (refreshTokenInterval) {
      clearInterval(refreshTokenInterval);
      refreshTokenInterval = null;
    }
  }

  function refreshToken() {
    safeClearInterval();

    const configs = window.ui.authSelectors.getConfigs();
    const definitions = window.ui.authSelectors.definitionsToAuthorize().toJS();
    const authorize = window.ui.authSelectors.authorized().toJS();

    if (Object.keys(authorize).length === 0) {
      console.warn('No authentication information available.');
      return;
    }

    let securityDefinitionName = Object.keys(authorize)[0];
    const currentDefinition = definitions.find(
      (item) => Object.keys(item)[0] == securityDefinitionName
    );

    if (!currentDefinition) {
      console.warn(
        'Authentication data is available, but does not match any registered authentication definition.'
      );
      return;
    }

    const refreshToken = authorize[securityDefinitionName].token.refresh_token;

    if (!refreshToken) {
      console.warn(
        'The previous request did not generate a valid refresh token.'
      );
      return;
    }

    const form = [];
    form.push('grant_type=refresh_token');
    form.push(
      [
        'client_id',
        '=',
        encodeURIComponent(configs.clientId).replace(/%20/g, '+'),
      ].join('')
    );
    form.push(
      [
        'client_secret',
        '=',
        encodeURIComponent(configs.clientSecret).replace(/%20/g, '+'),
      ].join('')
    );
    form.push(
      [
        'refresh_token',
        '=',
        encodeURIComponent(refreshToken).replace(/%20/g, '+'),
      ].join('')
    );

    console.log('Initiating new access token request.');
    window.ui.authActions.authorizeRequest({
      body: form.join('&'),
      securityDefinitionName,
      url: currentDefinition[securityDefinitionName].tokenUrl,
      auth: authorize[securityDefinitionName],
    });
  }

  window.ui.authActions.authorizeOauth2 = (
    (originalAuthorize) =>
    (...args) => {
      const result = originalAuthorize(...args);
      console.log('Authenticated user, monitoring access token expiration.');

      if (!refreshTokenInterval) {
        const clockSkew = 100;
        const expiresInMilliseconds =
          (result.payload.token.expires_in - clockSkew) * 1000;
        refreshTokenInterval = setInterval(refreshToken, expiresInMilliseconds);
      }

      return result;
    }
  )(window.ui.authActions.authorizeOauth2);

  window.ui.authActions.logout = (
    (originalLogout) =>
    (...args) => {
      const result = originalLogout(...args);
      console.log('user disconnected, interrupting refresh_token flow.');

      safeClearInterval();

      return result;
    }
  )(window.ui.authActions.logout);
})();
