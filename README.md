# SimTemplate
##Sam's super awesome template tool.
A WPF application to facilitate manual construction of templates for captures in the SimPrints database.

### TODOs
- [x] Scanner select drop down
- [ ] User guidance
  * Help icon
  * Provide a How To
- [ ] Image rotation
  * Need to check that x y angles are correct. Maybe we re-save the image properly in the DB?
  * Look briefly and if it's a huge pain in the ass ignore NEXT image (for now).
- [ ] REST calls from WPF
  * Query a DB server via REST to get images/templates
  * Appropriate recording of API key (shouldn't save in source code)
- [x] Load templated capture
  * Review template
  * Update template

The proposed tasks above necessitate the following API:

| Function | Send | Receive |
| --- | --- | --- |
| Get Untemplated Capture | Scanner Type | Image, Capture GUID |
| Get Templated Capture | Scanner Type | Image, Capture GUID, Template |
| Save Capture | Template, Capture GUID | *nothing* |
