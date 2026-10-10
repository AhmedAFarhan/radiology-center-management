-- Clinic image users can view images; only the local archive administrator can modify the archive.
-- AuthenticationEnabled must remain true. RegisteredUsers contains one account per person.
function IncomingHttpRequestFilter(method, uri, ip, username, httpHeaders)
  if username == 'archive-admin' and (ip == '127.0.0.1' or ip == '::1') then
    return true
  end
  if username == nil or username == '' or username == 'archive-admin' then
    return false
  end
  if method ~= 'GET' then
    return false
  end
  return uri == '/dicom-web' or string.sub(uri, 1, 11) == '/dicom-web/'
      or string.sub(uri, 1, 6) == '/ohif/'
      or uri == '/ohif'
      or uri == '/wado'
end
